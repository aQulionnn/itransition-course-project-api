namespace ItransitionCourseProject.Api.Features.Inventories.UpdateInventory;

public record UpdateInventoryRequest(
    string Title,
    string Description,
    string ImageUrl,
    bool IsPublic,
    Guid CategoryId
);