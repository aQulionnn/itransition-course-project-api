using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Tags.GetTags;

public class GetTagsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags", async (AppDbContext db) =>
        {
            var tags = await db.Tags
                .Select(t => new GetTagsResponse(
                    t.Id,
                    t.Name,
                    t.InventoryTags.Count(it => it.Inventory.IsPublic)
                ))
                .Where(t => t.Count > 0)
                .OrderByDescending(t => t.Count)
                .ToListAsync();

            return Results.Ok(tags);
        }).AllowAnonymous().WithTags("Tags");
    }
}