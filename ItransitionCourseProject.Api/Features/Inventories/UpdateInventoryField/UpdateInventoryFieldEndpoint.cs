using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.UpdateInventoryField;

public class UpdateInventoryFieldEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/inventories/{inventoryId:guid}/fields/{fieldId:guid}", async (
            Guid inventoryId,
            Guid fieldId,
            UpdateInventoryFieldRequest request,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inventory is null) return Results.NotFound();
            if (inventory.CreatorId != userId && !isAdmin) return Results.Forbid();

            var field = await db.InventoryFields
                .FirstOrDefaultAsync(f => f.Id == fieldId && f.InventoryId == inventoryId);
            if (field is null) return Results.NotFound();

            field.Title = request.Title;
            field.Description = request.Description;
            field.IsDisplayed = request.IsDisplayed;

            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization().WithTags("InventoryFields");
    }
}

public record UpdateInventoryFieldRequest(
    string Title,
    string Description,
    bool IsDisplayed
);