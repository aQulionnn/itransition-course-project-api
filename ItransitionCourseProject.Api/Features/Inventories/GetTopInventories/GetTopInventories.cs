using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Inventories.GetTopInventories;

public class GetTopInventoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/top", async (AppDbContext db) =>
        {
            var inventories = await db.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Items)
                .Where(i => i.IsPublic)
                .OrderByDescending(i => i.Items.Count())
                .Take(5)
                .Select(i => new GetTopInventoriesResponse(
                    i.Id,
                    i.Title,
                    i.Creator.UserName!,
                    i.CreatorId,
                    i.Items.Count()
                ))
                .ToListAsync();

            return Results.Ok(inventories);
        }).AllowAnonymous().WithTags("Inventories");
    }
}