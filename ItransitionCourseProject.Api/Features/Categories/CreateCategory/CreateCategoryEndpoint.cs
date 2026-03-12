using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionCourseProject.Api.Features.Categories.CreateCategory;

public class CreateCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categories", async ([FromBody] CreateCategoryRequest request, AppDbContext context) =>
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Results.Ok(category.Id);
        }).WithTags("Categories");
    }
}

public record CreateCategoryRequest(string Name);