using System.Security.Claims;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionCourseProject.Api.Features.Images.UploadImage;

public class UploadImageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/images/upload",
                async ([FromForm] IFormFile file, ClaimsPrincipal principal, ISender sender) =>
                {
                    var command = new UploadImageCommand(file, principal);
                    var response = await sender.Send(command);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .DisableAntiforgery()
            .WithTags("Images");
    }
}