namespace ItransitionCourseProject.Api.Features.Tags.GetTags;

public record GetTagsResponse(
    Guid Id,
    string Name,
    int Count
);