using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.AddInventoryAccess;

public class AddInventoryAccessEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventories/{id:guid}/access", async (
            Guid id,
            AddInventoryAccessRequest request,
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

            var alreadyExists = await db.InventoryAccesses
                .AnyAsync(a => a.InventoryId == id && a.UserId == request.UserId);

            if (alreadyExists)
                return Results.Conflict("User already has access");

            db.InventoryAccesses.Add(new InventoryAccess
            {
                InventoryId = id,
                UserId = request.UserId,
                Inventory = null!,
                User = null!
            });

            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithTags("Inventories");
    }
}