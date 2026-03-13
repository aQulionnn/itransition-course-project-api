using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Items.CreateItem;

public class CreateItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventories/{inventoryId:guid}/items", async (
            Guid inventoryId,
            CreateItemRequest request,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories
                .AsNoTracking()
                .Where(i => i.Id == inventoryId)
                .Select(i => new { i.CreatorId, i.IsPublic })
                .FirstOrDefaultAsync();

            if (inventory is null) return Results.NotFound();

            var hasAccess = isAdmin
                || inventory.CreatorId == userId
                || inventory.IsPublic
                || await db.InventoryAccesses.AnyAsync(a =>
                    a.InventoryId == inventoryId && a.UserId == userId);

            if (!hasAccess) return Results.Forbid();

            var customId = await GenerateCustomIdAsync(db, inventoryId);

            var duplicate = await db.Items.AnyAsync(i =>
                i.InventoryId == inventoryId && i.CustomId == customId);

            if (duplicate)
                return Results.Conflict("Generated Custom ID already exists. Please try again.");

            var itemId = Guid.NewGuid();

            var fieldValues = request.FieldValues.Select(fv => new ItemFieldValue
            {
                Id = Guid.NewGuid(),
                InventoryFieldId = fv.FieldId,
                Value = fv.Value,
                ItemId = itemId,
                Item = null!,
                InventoryField = null!
            }).ToList();

            var item = new Item
            {
                Id = itemId,
                CustomId = customId,
                InventoryId = inventoryId,
                CreatorId = userId,
                CreatedOnUtc = DateTime.UtcNow,
                Inventory = null!,
                Creator = null!,
                ItemFieldValues = fieldValues
            };

            db.Items.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/inventories/{inventoryId}/items/{item.Id}",
                new { id = item.Id, customId });
        }).RequireAuthorization().WithTags("Items");
    }

    private static async Task<string> GenerateCustomIdAsync(AppDbContext db, Guid inventoryId)
    {
        var format = await db.InventoryIdFormats
            .AsNoTracking()
            .Include(f => f.Elements)
            .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

        if (format is null || format.Elements.Count == 0)
            return Guid.NewGuid().ToString("N")[..8].ToUpper();

        var parts = new List<string>();
        foreach (var el in format.Elements.OrderBy(e => e.Order))
            parts.Add(await ResolveElementAsync(el, db, inventoryId));

        return string.Concat(parts);
    }

    private static async Task<string> ResolveElementAsync(
        InventoryIdElement el, AppDbContext db, Guid inventoryId)
    {
        if (el.Type == ElementType.Sequence)
        {
            var count = await db.Items.CountAsync(i => i.InventoryId == inventoryId);
            return (count + 1).ToString();
        }
        return ResolveStaticElement(el);
    }

    private static string ResolveStaticElement(InventoryIdElement el) => el.Type switch
    {
        ElementType.FixedText => el.Text ?? string.Empty,
        ElementType.Random20Bit => ApplyPadding(Random.Shared.Next(0, 1048576).ToString(), el.Padding),
        ElementType.Random32Bit => ApplyPadding(Random.Shared.Next(0, int.MaxValue).ToString(), el.Padding),
        ElementType.Random6Digit => ApplyPadding(Random.Shared.Next(0, 1000000).ToString(), el.Padding > 0 ? el.Padding : 6),
        ElementType.Random9Digit => ApplyPadding(Random.Shared.Next(0, 1000000000).ToString(), el.Padding > 0 ? el.Padding : 9),
        ElementType.Guid => Guid.NewGuid().ToString("N").ToUpper(),
        ElementType.DateTime => DateTime.UtcNow.ToString(el.DateFormat ?? "yyyyMMdd"),
        _ => string.Empty
    };

    private static string ApplyPadding(string value, int padding) =>
        padding > 0 ? value.PadLeft(padding, '0') : value;
}

public record CreateItemRequest(List<FieldValueRequest> FieldValues);
public record FieldValueRequest(Guid FieldId, string Value);