using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItransitionCourseProject.Api.Features.Inventories.SaveIdFormat;

public class SaveIdFormatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/inventories/{inventoryId:guid}/id-format", async (
            Guid inventoryId,
            SaveIdFormatRequest request,
            HttpContext ctx,
            AppDbContext db) =>
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = ctx.User.IsInRole("Admin");

            var inventory = await db.Inventories
                .AsNoTracking()
                .Where(i => i.Id == inventoryId)
                .Select(i => new { i.CreatorId })
                .FirstOrDefaultAsync();

            if (inventory is null) return Results.NotFound();
            if (inventory.CreatorId != userId && !isAdmin) return Results.Forbid();

            var existing = await db.InventoryIdFormats
                .Include(f => f.Elements)
                .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

            if (existing is not null)
            {
                db.InventoryIdElements.RemoveRange(existing.Elements);
                existing.Elements = BuildElements(request.Elements, existing.Id);
            }
            else
            {
                var format = new InventoryIdFormat
                {
                    Id = Guid.NewGuid(),
                    InventoryId = inventoryId,
                    Inventory = null!,
                    Elements = []
                };
                format.Elements = BuildElements(request.Elements, format.Id);
                db.InventoryIdFormats.Add(format);
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization().WithTags("IdFormat");
    }

    private static List<InventoryIdElement> BuildElements(
        List<SaveIdElementRequest> requests, Guid formatId) =>
        requests.Select((r, i) => new InventoryIdElement
        {
            Id = Guid.NewGuid(),
            Type = Enum.Parse<ElementType>(r.Type),
            Order = i,
            Text = r.Text,
            Padding = r.Padding,
            DateFormat = r.DateFormat,
            FormatId = formatId,
            Format = null!
        }).ToList();
}

public record SaveIdFormatRequest(List<SaveIdElementRequest> Elements);
public record SaveIdElementRequest(string Type, string? Text, int Padding, string? DateFormat);