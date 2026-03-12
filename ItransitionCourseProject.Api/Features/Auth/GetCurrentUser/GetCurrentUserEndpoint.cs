using System.Security.Claims;
using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Features.Auth.GetCurrentUser;

public class GetCurrentUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", async (ClaimsPrincipal principal, UserManager<AppUser> userManager) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();
            
            var user = await userManager.GetUserAsync(principal);
            
            if (user is null)
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(user);

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                IsAdmin = roles.Contains("Admin")
            });
        }).WithTags("Auth");
    }
}