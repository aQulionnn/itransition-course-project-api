using Carter;
using Microsoft.AspNetCore.Identity;
using ItransitionCourseProject.Api.Models;

namespace ItransitionCourseProject.Api.Features.Auth.Logout;

public class LogoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", async (SignInManager<AppUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }).WithTags("Auth");
    }
}