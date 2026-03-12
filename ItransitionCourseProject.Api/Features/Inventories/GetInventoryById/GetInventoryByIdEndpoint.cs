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
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Where(i =>
                    isAdmin ||
                    i.IsPublic ||
                    i.CreatorId == userId ||
                    i.InventoryAccesses.Any(a => a.UserId == userId))
                .Select(i => new GetInventoryByIdResponse(
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    i.IsPublic,
                    i.CreatorId,
                    i.Creator.UserName!,
                    i.Category.Name,
                    i.Items.Count(),
                    i.InventoryTags.Select(t => t.Tag.Name).ToList(),
                    i.InventoryFields.Select(f => new InventoryFieldResponse(
                        f.Id,
                        f.Title,
                        f.Description,
                        f.Type.ToString(),
                        f.IsDisplayed
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (inventory is null)
                return Results.NotFound();

            return Results.Ok(inventory);
        }).AllowAnonymous().WithTags("Inventories");
    }
}