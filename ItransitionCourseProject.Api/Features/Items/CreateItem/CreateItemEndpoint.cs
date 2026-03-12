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

            var duplicate = await db.Items.AnyAsync(i =>
                i.InventoryId == inventoryId && i.CustomId == request.CustomId);

            if (duplicate) return Results.Conflict("Custom ID already exists in this inventory.");

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
                CustomId = request.CustomId,
                InventoryId = inventoryId,
                CreatorId = userId,
                CreatedOnUtc = DateTime.UtcNow,
                Inventory = null!,
                Creator = null!,
                ItemFieldValues = fieldValues
            };

            db.Items.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/inventories/{inventoryId}/items/{item.Id}", item.Id);
        }).RequireAuthorization().WithTags("Items");
    }
}

public record CreateItemRequest(
    string CustomId,
    List<FieldValueRequest> FieldValues
);

public record FieldValueRequest(Guid FieldId, string Value);