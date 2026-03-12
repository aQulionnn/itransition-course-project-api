namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoryById;

public record InventoryFieldResponse(
    Guid Id,
    string Title,
    string Description,
    string Type,
    bool IsDisplayed
);