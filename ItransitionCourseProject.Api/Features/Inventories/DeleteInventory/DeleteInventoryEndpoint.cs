using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.DeleteInventory;

public class DeleteInventoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/inventories/{id:guid}", async (
            Guid id,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.Id == id);

            if (inventory is null)
                return Results.NotFound();

            if (!isAdmin && inventory.CreatorId != userId)
                return Results.Forbid();

            db.Inventories.Remove(inventory);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithTags("Inventories");
    }
}