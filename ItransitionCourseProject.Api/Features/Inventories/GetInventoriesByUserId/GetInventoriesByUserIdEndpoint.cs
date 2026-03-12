// Features/Inventories/GetInventoriesByUserId/GetInventoriesByUserIdEndpoint.cs
using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoriesByUserId;

public class GetInventoriesByUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId}/inventories", async (
            string userId,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var currentUserId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventories = await db.Inventories
                .Include(i => i.Category)
                .Include(i => i.Items)
                .Include(i => i.InventoryTags)
                .ThenInclude(t => t.Tag)
                .Where(i =>
                    i.CreatorId == userId &&
                    (
                        isAdmin ||
                        i.IsPublic ||
                        i.CreatorId == currentUserId ||
                        i.InventoryAccesses.Any(a => a.UserId == currentUserId)
                    ))
                .Select(i => new GetInventoriesByUserIdResponse(
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    i.IsPublic,
                    i.Category.Name,
                    i.Items.Count(),
                    i.InventoryTags.Select(t => t.Tag.Name).ToList()
                ))
                .ToListAsync();

            return Results.Ok(inventories);
        }).AllowAnonymous().WithTags("Inventories");
    }
}