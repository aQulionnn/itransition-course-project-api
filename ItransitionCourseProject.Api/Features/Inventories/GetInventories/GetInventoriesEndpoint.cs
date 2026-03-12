using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.GetInventories;

public class GetInventoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories", async (
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventories = await db.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.Items)
                .Include(i => i.InventoryTags)
                .ThenInclude(t => t.Tag)
                .Where(i =>
                    isAdmin ||
                    i.IsPublic ||
                    i.CreatorId == userId ||
                    i.InventoryAccesses.Any(a => a.UserId == userId))
                .Select(i => new GetInventoriesResponse(
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    i.IsPublic,
                    i.CreatorId,
                    i.Creator.UserName!,
                    i.Category.Name,
                    i.Items.Count(),
                    i.InventoryTags.Select(t => t.Tag.Name).ToList()
                ))
                .ToListAsync();

            return Results.Ok(inventories);
        }).AllowAnonymous().WithTags("Inventories");
    }
}