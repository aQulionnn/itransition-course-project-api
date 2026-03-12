using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoryById;

public class GetInventoryByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/{id:guid}", async (
            Guid id,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.Items)
                .Include(i => i.InventoryTags)
                .ThenInclude(t => t.Tag)
                .Include(i => i.InventoryFields)
                .Where(i =>
                    isAdmin ||
                    i.IsPublic ||
                    i.CreatorId == userId ||
                    i.InventoryAccesses.Any(a => a.UserId == userId))
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory is null)
                return Results.NotFound();

            return Results.Ok(new GetInventoryByIdResponse(
                inventory.Id,
                inventory.Title,
                inventory.Description,
                inventory.ImageUrl,
                inventory.IsPublic,
                inventory.CreatorId,
                inventory.Creator.UserName!,
                inventory.Category.Name,
                inventory.Items.Count(),
                inventory.InventoryTags.Select(t => t.Tag.Name).ToList(),
                inventory.InventoryFields.Select(f => new InventoryFieldResponse(
                    f.Id,
                    f.Title,
                    f.Description,
                    f.Type.ToString(),
                    f.IsDisplayed
                )).ToList()
            ));
        }).AllowAnonymous().WithTags("Inventories");
    }
}