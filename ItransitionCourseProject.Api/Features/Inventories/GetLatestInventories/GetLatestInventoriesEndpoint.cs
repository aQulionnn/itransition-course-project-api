// Features/Inventories/GetLatestInventories/GetLatestInventoriesEndpoint.cs
using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Inventories.GetLatestInventories;

public class GetLatestInventoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/latest", async (AppDbContext db) =>
        {
            var inventories = await db.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Where(i => i.IsPublic)
                .OrderByDescending(i => i.CreatedOnUtc)
                .Take(10)
                .Select(i => new GetLatestInventoriesResponse(
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    i.Creator.UserName!,
                    i.CreatorId,
                    i.Category.Name
                ))
                .ToListAsync();

            return Results.Ok(inventories);
        }).AllowAnonymous().WithTags("Inventories");
    }
}