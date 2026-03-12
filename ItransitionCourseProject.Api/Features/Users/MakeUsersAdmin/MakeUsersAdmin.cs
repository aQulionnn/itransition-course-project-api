using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionCourseProject.Api.Features.Users.MakeUsersAdmin;

public class MakeAdminsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/users/make-admins", async (
            [FromBody] List<string> ids,
            UserManager<AppUser> userManager) =>
        {
            foreach (var id in ids)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null) continue;
                await userManager.AddToRoleAsync(user, "Admin");
            }

            return Results.NoContent();
        }).WithTags("Users");
    }
}