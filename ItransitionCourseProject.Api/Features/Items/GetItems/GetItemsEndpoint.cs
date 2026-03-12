using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Items.GetItems;

public class GetItemsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/{inventoryId:guid}/items", async (
            Guid inventoryId,
            AppDbContext db) =>
        {
            var exists = await db.Inventories.AnyAsync(i => i.Id == inventoryId);
            if (!exists) return Results.NotFound();

            var items = await db.Items
                .AsNoTracking()
                .Where(i => i.InventoryId == inventoryId)
                .Select(i => new GetItemsResponse(
                    i.Id,
                    i.CustomId,
                    i.Creator.UserName!,
                    i.CreatedOnUtc,
                    i.ItemFieldValues.Select(fv => new FieldValueResponse(
                        fv.InventoryFieldId,
                        fv.InventoryField.Title,
                        fv.Value,
                        fv.InventoryField.IsDisplayed
                    )).ToList()
                ))
                .ToListAsync();

            return Results.Ok(items);
        }).AllowAnonymous().WithTags("Items");
    }
}

public record GetItemsResponse(
    Guid Id,
    string CustomId,
    string CreatorName,
    DateTime CreatedOnUtc,
    List<FieldValueResponse> FieldValues
);
 
public record FieldValueResponse(Guid FieldId, string FieldTitle, string Value, bool IsDisplayed);