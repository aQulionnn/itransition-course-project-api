using Carter;
using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Inventories.GetIdFormat;

public class GetIdFormatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventories/{inventoryId:guid}/id-format", async (
            Guid inventoryId,
            AppDbContext db) =>
        {
            var format = await db.InventoryIdFormats
                .AsNoTracking()
                .Include(f => f.Elements)
                .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

            if (format is null)
                return Results.Ok(new GetIdFormatResponse(null, [], string.Empty));

            var elements = format.Elements
                .OrderBy(e => e.Order)
                .Select(e => new IdElementResponse(
                    e.Id, e.Type.ToString(), e.Order, e.Text, e.Padding, e.DateFormat))
                .ToList();

            var preview = BuildPreview(format.Elements);

            return Results.Ok(new GetIdFormatResponse(format.Id, elements, preview));
        }).AllowAnonymous().WithTags("IdFormat");
    }

    private static string BuildPreview(IEnumerable<InventoryIdElement> elements)
    {
        var parts = elements.OrderBy(e => e.Order).Select(ResolveElement);
        return string.Concat(parts);
    }

    private static string ResolveElement(InventoryIdElement el) => el.Type switch
    {
        ElementType.FixedText => el.Text ?? string.Empty,
        ElementType.Random20Bit => ApplyPadding(Random.Shared.Next(0, 1048576).ToString(), el.Padding),
        ElementType.Random32Bit => ApplyPadding(Random.Shared.Next(0, int.MaxValue).ToString(), el.Padding),
        ElementType.Random6Digit => ApplyPadding(Random.Shared.Next(0, 1000000).ToString(),
            el.Padding > 0 ? el.Padding : 6),
        ElementType.Random9Digit => ApplyPadding(Random.Shared.Next(0, 1000000000).ToString(),
            el.Padding > 0 ? el.Padding : 9),
        ElementType.Guid => Guid.NewGuid().ToString("N").ToUpper(),
        ElementType.DateTime => DateTime.UtcNow.ToString(el.DateFormat ?? "yyyyMMdd"),
        ElementType.Sequence => "1",
        _ => string.Empty
    };

    private static string ApplyPadding(string value, int padding) =>
        padding > 0 ? value.PadLeft(padding, '0') : value;
}

public record GetIdFormatResponse(Guid? FormatId, List<IdElementResponse> Elements, string Preview);

public record IdElementResponse(Guid Id, string Type, int Order, string? Text, int Padding, string? DateFormat);