using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Users.GetUsers;

public class GetUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (UserManager<AppUser> userManager) =>
            {
                var users = await userManager.Users
                    .Select(u => new GetUsersResponse(u.Id, u.UserName!, u.Email!, u.IsBlocked))
                    .ToListAsync();

                return Results.Ok(users);
            })
            .RequireAuthorization("Admin")
            .WithTags("Users");
    }
}