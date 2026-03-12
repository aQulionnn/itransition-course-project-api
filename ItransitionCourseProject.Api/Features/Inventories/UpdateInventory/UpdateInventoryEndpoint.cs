using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.UpdateInventory;

public class UpdateInventoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/inventories/{id:guid}", async (
            Guid id,
            UpdateInventoryRequest request,
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

            inventory.Title = request.Title;
            inventory.Description = request.Description;
            inventory.ImageUrl = request.ImageUrl;
            inventory.IsPublic = request.IsPublic;
            inventory.CategoryId = request.CategoryId;

            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithTags("Inventories");
    }
}