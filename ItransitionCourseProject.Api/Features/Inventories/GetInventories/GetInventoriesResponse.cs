namespace ItransitionCourseProject.Api.Features.Inventories.GetInventories;

public record GetInventoriesResponse(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    bool IsPublic,
    string CreatorId,
    string CreatorName,
    string CategoryName,
    int ItemsCount,
    IList<string> Tags
);