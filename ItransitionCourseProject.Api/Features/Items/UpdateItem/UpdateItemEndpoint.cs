using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Items.UpdateItem;

public class UpdateItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/inventories/{inventoryId:guid}/items/{itemId:guid}", async (
            Guid inventoryId,
            Guid itemId,
            UpdateItemRequest request,
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

            var item = await db.Items
                .Include(i => i.ItemFieldValues)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.InventoryId == inventoryId);

            if (item is null) return Results.NotFound();

            var duplicate = await db.Items.AnyAsync(i =>
                i.InventoryId == inventoryId && i.CustomId == request.CustomId && i.Id != itemId);

            if (duplicate) return Results.Conflict("Custom ID already exists in this inventory.");

            item.CustomId = request.CustomId;
            item.ModifiedOnUtc = DateTime.UtcNow;

            db.ItemFieldValues.RemoveRange(item.ItemFieldValues);

            foreach (var fv in request.FieldValues)
            {
                db.ItemFieldValues.Add(new ItemFieldValue
                {
                    Id = Guid.NewGuid(),
                    InventoryFieldId = fv.FieldId,
                    Value = fv.Value,
                    ItemId = item.Id,
                    Item = null!,
                    InventoryField = null!
                });
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization().WithTags("Items");
    }
}

public record UpdateItemRequest(
    string CustomId,
    List<FieldValueRequest> FieldValues
);
 
public record FieldValueRequest(Guid FieldId, string Value);