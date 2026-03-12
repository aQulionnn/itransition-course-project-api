namespace ItransitionCourseProject.Api.Features.Users.GetUsers;

public record GetUsersResponse(
    string Id,
    string UserName,
    string Email,
    bool IsBlocked
);