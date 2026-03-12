using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.AddInventoryField;

public class AddInventoryFieldEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventories/{inventoryId:guid}/fields", async (
            Guid inventoryId,
            AddInventoryFieldRequest request,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories
                .Include(i => i.InventoryFields)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory is null) return Results.NotFound();
            if (inventory.CreatorId != userId && !isAdmin) return Results.Forbid();

            if (!Enum.TryParse<Models.Type>(request.Type, out var fieldType))
                return Results.BadRequest("Invalid field type.");

            var validationError = ValidateFieldLimit(inventory.InventoryFields, fieldType);
            if (validationError is not null) return Results.BadRequest(validationError);

            var field = new InventoryField
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Type = fieldType,
                IsDisplayed = request.IsDisplayed,
                InventoryId = inventoryId,
                Inventory = null!
            };

            db.InventoryFields.Add(field);
            await db.SaveChangesAsync();

            return Results.Created($"/api/inventories/{inventoryId}/fields/{field.Id}", field.Id);
        }).RequireAuthorization().WithTags("InventoryFields");
    }

    private static string? ValidateFieldLimit(IEnumerable<InventoryField> fields, Models.Type type)
    {
        var count = fields.Count(f => f.Type == type);
        return count >= 3 ? $"Maximum 3 fields of type '{type}' allowed." : null;
    }
}

public record AddInventoryFieldRequest(
    string Title,
    string Description,
    string Type,
    bool IsDisplayed
);