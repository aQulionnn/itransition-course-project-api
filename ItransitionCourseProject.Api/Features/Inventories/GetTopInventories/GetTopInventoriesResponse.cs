namespace ItransitionCourseProject.Api.Features.Inventories.GetTopInventories;

public record GetTopInventoriesResponse(
    Guid Id,
    string Title,
    string CreatorName,
    string CreatorId,
    int ItemsCount
);