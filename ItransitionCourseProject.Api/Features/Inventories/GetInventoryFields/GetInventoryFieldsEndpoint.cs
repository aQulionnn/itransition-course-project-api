using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoryFields;

public class GetInventoryFieldsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/{inventoryId:guid}/fields", async (
            Guid inventoryId,
            AppDbContext db) =>
        {
            var exists = await db.Inventories.AnyAsync(i => i.Id == inventoryId);
            if (!exists) return Results.NotFound();

            var fields = await db.InventoryFields
                .Where(f => f.InventoryId == inventoryId)
                .Select(f => new GetInventoryFieldsResponse(
                    f.Id, f.Title, f.Description, f.Type.ToString(), f.IsDisplayed))
                .ToListAsync();

            return Results.Ok(fields);
        }).WithTags("InventoryFields");
    }
}

public record GetInventoryFieldsResponse(
    Guid Id,
    string Title,
    string Description,
    string Type,
    bool IsDisplayed
);