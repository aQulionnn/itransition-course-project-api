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
            var inventoryId = Guid.NewGuid();

            var inventoryTags = await BuildTagsAsync(db, request.Tags, inventoryId);

            var inventory = new Inventory
            {
                Id = inventoryId,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsPublic = request.IsPublic,
                CreatorId = userId,
                CategoryId = request.CategoryId,
                Creator = null!,
                Category = null!,
                InventoryTags = inventoryTags
            };

            db.Inventories.Add(inventory);
            await db.SaveChangesAsync();

            return Results.Created($"/api/inventories/{inventory.Id}", inventory.Id);
        }).RequireAuthorization().WithTags("Inventories");
    }

    private static async Task<List<InventoryTag>> BuildTagsAsync(
        AppDbContext db, IEnumerable<string> tagNames, Guid inventoryId)
    {
        var tags = new List<InventoryTag>();
        foreach (var name in tagNames)
        {
            var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == name)
                      ?? new Tag { Id = Guid.NewGuid(), Name = name };

            tags.Add(new InventoryTag
            {
                InventoryId = inventoryId,
                TagId = tag.Id,
                Tag = tag,
                Inventory = null!
            });
        }
        return tags;
    }
}