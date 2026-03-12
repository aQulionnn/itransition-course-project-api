using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Categories.GetCategories;

public class GetCategoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async (AppDbContext db) =>
        {
            var categories = await db.Categories
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Results.Ok(categories);
        }).AllowAnonymous().WithTags("Categories");
    }
}