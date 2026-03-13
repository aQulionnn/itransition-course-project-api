using System.Security.Claims;
using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Features.Auth.MakeCurrentUserAdmin;

public class MakeCurrentUserAdminEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dev/make-admin", async (ClaimsPrincipal principal, UserManager<AppUser> userManager) =>
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return Results.Unauthorized();
                
                var user = await userManager.GetUserAsync(principal);
                
                if (user is null)
                    return Results.Unauthorized();
                
                await userManager.AddToRoleAsync(user, "Admin");
                
                return Results.Ok();
            })
            .WithTags("Dev");
    }
}