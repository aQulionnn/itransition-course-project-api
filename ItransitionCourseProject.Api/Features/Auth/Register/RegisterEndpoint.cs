using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Features.Auth.Register;

public class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegisterRequest request,
            UserManager<AppUser> userManager) =>
        {
            var existing = await userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
                return Results.Conflict("Email already exists");

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            await userManager.AddToRoleAsync(user, "User");
            await userManager.AddToRoleAsync(user, "Admin");

            return Results.Ok();
        }).AllowAnonymous().WithTags("Auth");
    }
}

public record RegisterRequest(string Email, string UserName, string Password);