namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoryById;

public record GetInventoryByIdResponse(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    bool IsPublic,
    string CreatorId,
    string CreatorName,
    string CategoryName,
    int ItemsCount,
    IList<string> Tags,
    IList<InventoryFieldResponse> Fields
);