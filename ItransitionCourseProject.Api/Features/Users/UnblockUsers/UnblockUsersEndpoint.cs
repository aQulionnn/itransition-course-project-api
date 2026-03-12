using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Users.UnblockUsers;

public class UnblockUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/users/unblock", async (
            [FromBody] List<string> ids,
            AppDbContext context) =>
        {
            await context.Users
                .Where(u => ids.Contains(u.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsBlocked, false));

            return Results.NoContent();
        }).WithTags("Users");
    }
}