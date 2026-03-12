namespace ItransitionCourseProject.Api.Features.Inventories.GetInventoriesByUserId;

public record GetInventoriesByUserIdResponse(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    bool IsPublic,
    string CategoryName,
    int ItemsCount,
    IList<string> Tags
);