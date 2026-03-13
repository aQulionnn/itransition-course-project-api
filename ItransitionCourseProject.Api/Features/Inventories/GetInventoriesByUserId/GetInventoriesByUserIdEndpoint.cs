using Carter;
using ItransitionCourseProject.Api.Data;
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

            var user = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new { u.UserName })
                .FirstOrDefaultAsync();

            if (user is null) return Results.NotFound();

            var inventories = await db.Inventories
                .AsNoTracking()
                .Where(i =>
                    i.CreatorId == userId &&
                    (isAdmin ||
                     i.IsPublic ||
                     i.CreatorId == currentUserId ||
                     i.InventoryAccesses.Any(a => a.UserId == currentUserId)))
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

            return Results.Ok(new { userName = user.UserName, inventories });
        }).AllowAnonymous().WithTags("Inventories");
    }
}