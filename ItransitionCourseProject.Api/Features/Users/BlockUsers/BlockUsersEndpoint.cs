using Carter;
using ItransitionCourseProject.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Features.Users.BlockUsers;

public class BlockUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/users/block", async ([FromBody] List<string> ids, AppDbContext context) =>
            {
                await context.Users
                    .Where(u => ids.Contains(u.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsBlocked, true));

                return Results.NoContent();
            })
            .WithTags("Users");
    }
}