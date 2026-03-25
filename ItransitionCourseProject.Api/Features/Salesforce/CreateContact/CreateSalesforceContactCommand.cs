// Features/Salesforce/CreateContact/CreateSalesforceContactCommand.cs
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ItransitionCourseProject.Api.Common;
using ItransitionCourseProject.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Features.Salesforce.CreateContact;

public record CreateSalesforceContactCommand(
    ClaimsPrincipal Principal,
    string FirstName,
    string LastName,
    string Phone,
    string Company
) : IRequest<Result<CreateSalesforceContactResponse>>;

public record CreateSalesforceContactResponse(string AccountId, string ContactId);

public sealed class CreateSalesforceContactCommandHandler(
    UserManager<AppUser> userManager,
    IConfiguration config,
    HttpClient httpClient) : IRequestHandler<CreateSalesforceContactCommand, Result<CreateSalesforceContactResponse>>
{
    public async Task<Result<CreateSalesforceContactResponse>> Handle(
        CreateSalesforceContactCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(request.Principal);
        if (user is null)
            return Result<CreateSalesforceContactResponse>.Failure(401, new Error("Unauthorized", null));

        // get access token
        var tokenResponse = await GetAccessTokenAsync(cancellationToken);
        if (tokenResponse is null)
            return Result<CreateSalesforceContactResponse>.Failure(502, new Error("Failed to authenticate with Salesforce", null));

        var instanceUrl = tokenResponse.Value.InstanceUrl;
        var accessToken = tokenResponse.Value.AccessToken;

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // create Account
        var accountPayload = JsonSerializer.Serialize(new { Name = request.Company });
        var accountResponse = await httpClient.PostAsync(
            $"{instanceUrl}/services/data/v59.0/sobjects/Account",
            new StringContent(accountPayload, Encoding.UTF8, "application/json"),
            cancellationToken);

        if (!accountResponse.IsSuccessStatusCode)
            return Result<CreateSalesforceContactResponse>.Failure(502, new Error("Failed to create Salesforce Account", null));

        var accountJson = await accountResponse.Content.ReadAsStringAsync(cancellationToken);
        var accountId = JsonDocument.Parse(accountJson).RootElement.GetProperty("id").GetString()!;

        // create Contact linked to Account
        var contactPayload = JsonSerializer.Serialize(new
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Email = user.Email,
            AccountId = accountId
        });

        var contactResponse = await httpClient.PostAsync(
            $"{instanceUrl}/services/data/v59.0/sobjects/Contact",
            new StringContent(contactPayload, Encoding.UTF8, "application/json"),
            cancellationToken);

        if (!contactResponse.IsSuccessStatusCode)
            return Result<CreateSalesforceContactResponse>.Failure(502, new Error("Failed to create Salesforce Contact", null));

        var contactJson = await contactResponse.Content.ReadAsStringAsync(cancellationToken);
        var contactId = JsonDocument.Parse(contactJson).RootElement.GetProperty("id").GetString()!;

        return Result<CreateSalesforceContactResponse>.Success(201, new CreateSalesforceContactResponse(accountId, contactId));
    }

    private async Task<(string AccessToken, string InstanceUrl)?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var domain = config["Salesforce:Domain"]!;
        var clientId = config["Salesforce:ClientId"]!;
        var clientSecret = config["Salesforce:ClientSecret"]!;

        var form = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        ]);

        var response = await httpClient.PostAsync($"{domain}/services/oauth2/token", form, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var accessToken = json.RootElement.GetProperty("access_token").GetString()!;
        var instanceUrl = json.RootElement.GetProperty("instance_url").GetString()!;

        return (accessToken, instanceUrl);
    }
}