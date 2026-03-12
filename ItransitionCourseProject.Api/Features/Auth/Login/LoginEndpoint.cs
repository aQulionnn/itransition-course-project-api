using System.Security.Claims;
using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Features.Auth.Login;

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            UserManager<AppUser> userManager,
            HttpContext ctx) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Unauthorized();

            if (user.IsBlocked)
                return Results.Forbid();

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.UserName!),
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme));

            return Results.SignIn(principal, authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
        }).AllowAnonymous().WithTags("Auth");
    }
}

public record LoginRequest(string Email, string Password);