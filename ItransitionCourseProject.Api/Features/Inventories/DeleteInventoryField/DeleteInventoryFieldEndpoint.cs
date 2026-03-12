using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.DeleteInventoryField;

public class DeleteInventoryFieldEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/inventories/{inventoryId:guid}/fields/{fieldId:guid}", async (
            Guid inventoryId,
            Guid fieldId,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inventory is null) return Results.NotFound();
            if (inventory.CreatorId != userId && !isAdmin) return Results.Forbid();

            var field = await db.InventoryFields
                .FirstOrDefaultAsync(f => f.Id == fieldId && f.InventoryId == inventoryId);
            if (field is null) return Results.NotFound();

            db.InventoryFields.Remove(field);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization().WithTags("InventoryFields");
    }
}