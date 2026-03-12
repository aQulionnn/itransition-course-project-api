namespace ItransitionCourseProject.Api.Features.Inventories.CreateInventory;

public record CreateInventoryRequest(
    string Title,
    string Description,
    string ImageUrl,
    bool IsPublic,
    Guid CategoryId,
    IList<string> Tags
);