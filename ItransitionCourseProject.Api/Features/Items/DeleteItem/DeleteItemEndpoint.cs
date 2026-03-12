using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Items.DeleteItem;

public class DeleteItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/inventories/{inventoryId:guid}/items/{itemId:guid}", async (
            Guid inventoryId,
            Guid itemId,
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
                .FirstOrDefaultAsync(i => i.Id == itemId && i.InventoryId == inventoryId);

            if (item is null) return Results.NotFound();

            db.Items.Remove(item);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization().WithTags("Items");
    }
}