using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ItransitionCourseProject.Api.Common;
using MediatR;

namespace ItransitionCourseProject.Api.Features.Images.UploadImage;

public sealed class UploadImageCommandHandler(BlobServiceClient blobServiceClient)
    : IRequestHandler<UploadImageCommand, Result<UploadImageResponse>>
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;

    public async Task<Result<UploadImageResponse>> Handle(UploadImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Result<UploadImageResponse>.Failure(401, new Error("Unauthorized", null));

        var container = _blobServiceClient.GetBlobContainerClient("images");
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobName = $"users/{userId}/{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(request.File.OpenReadStream(), new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = request.File.ContentType
            }
        }, cancellationToken);

        var response = new UploadImageResponse(blob.Uri.ToString());
        return Result<UploadImageResponse>.Success(201, response);
    }
}

public record UploadImageCommand(IFormFile File, ClaimsPrincipal Principal) : IRequest<Result<UploadImageResponse>>;