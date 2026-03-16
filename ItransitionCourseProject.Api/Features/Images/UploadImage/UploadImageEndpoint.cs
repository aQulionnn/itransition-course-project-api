using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionCourseProject.Api.Features.Images.UploadImage;

public class UploadImageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/images/upload", async ([FromForm] IFormFile file, ClaimsPrincipal principal,
                UserManager<AppUser> userManager, BlobServiceClient blobServiceClient) =>
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return Results.Unauthorized();

                var user = await userManager.GetUserAsync(principal);
                if (user is null)
                    return Results.Unauthorized();

                var container = blobServiceClient.GetBlobContainerClient("images");

                var blobName = $"users/{userId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blob = container.GetBlobClient(blobName);

                await blob.UploadAsync(file.OpenReadStream(), new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

                return Results.Ok(new { url = blob.Uri });
            })
            .DisableAntiforgery()
            .WithTags("Images");
    }
}