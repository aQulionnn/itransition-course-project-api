using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.CreateInventory;

public class CreateInventoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventories", async (
            CreateInventoryRequest request,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsPublic = request.IsPublic,
                CreatorId = userId,
                CategoryId = request.CategoryId,
                Creator = null!,
                Category = null!
            };

            foreach (var tagName in request.Tags)
            {
                var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                          ?? new Tag { Id = Guid.NewGuid(), Name = tagName };

                inventory.InventoryTags = inventory.InventoryTags.Append(new InventoryTag
                {
                    InventoryId = inventory.Id,
                    TagId = tag.Id,
                    Tag = tag,
                    Inventory = null!
                }).ToList();
            }

            db.Inventories.Add(inventory);
            await db.SaveChangesAsync();

            return Results.Created($"/api/inventories/{inventory.Id}", inventory.Id);
        }).WithTags("Inventories");
    }
}