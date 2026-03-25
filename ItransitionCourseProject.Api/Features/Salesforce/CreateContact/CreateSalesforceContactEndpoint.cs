// Features/Salesforce/CreateContact/CreateSalesforceContactEndpoint.cs
using System.Security.Claims;
using Carter;
using MediatR;

namespace ItransitionCourseProject.Api.Features.Salesforce.CreateContact;

public class CreateSalesforceContactEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/salesforce/contact", async (
            CreateSalesforceContactRequest request,
            ClaimsPrincipal principal,
            ISender sender) =>
        {
            var command = new CreateSalesforceContactCommand(
                principal,
                request.FirstName,
                request.LastName,
                request.Phone,
                request.Company);

            var result = await sender.Send(command);
            return Results.Json(result.Data, statusCode: result.StatusCode);
        }).WithTags("Salesforce");
    }
}

public record CreateSalesforceContactRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Company
);