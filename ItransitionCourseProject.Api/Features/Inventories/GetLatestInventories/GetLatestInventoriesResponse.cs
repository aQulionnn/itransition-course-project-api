namespace ItransitionCourseProject.Api.Features.Inventories.GetLatestInventories;

public record GetLatestInventoriesResponse(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    string CreatorName,
    string CreatorId,
    string CategoryName
);