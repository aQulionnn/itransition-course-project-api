using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionCourseProject.Api.Features.Users.RemoveAdminFromUsers;

public class RemoveAdminFromUsers : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/users/remove-admins", async (
            [FromBody] List<string> ids,
            UserManager<AppUser> userManager) =>
        {
            foreach (var id in ids)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null) continue;
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }

            return Results.NoContent();
        }).WithTags("Users");
    }
}