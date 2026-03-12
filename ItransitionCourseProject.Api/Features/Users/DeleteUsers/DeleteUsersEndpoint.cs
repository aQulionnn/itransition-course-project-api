using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Users.DeleteUsers;

public class DeleteUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users", async (
            [FromBody] List<string> ids,
            AppDbContext db) =>
        {
            await db.Users
                .Where(u => ids.Contains(u.Id))
                .ExecuteDeleteAsync();

            return Results.NoContent();
        }).WithTags("Users");
    }
}