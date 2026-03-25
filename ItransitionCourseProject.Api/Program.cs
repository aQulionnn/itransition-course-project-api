using Azure.Identity;
using Carter;
using DotNetEnv;
using ItransitionCourseProject.Api.Extensions;
using ItransitionCourseProject.Api.Features.Salesforce.CreateContact;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddFeatureManagement();

builder.WebHost.UseKestrel(options => { options.AddServerHeader = false; });

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVault:Uri"]!);
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage"));
});

builder.Services.AddHttpClient<CreateSalesforceContactCommandHandler>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.MaxDepth = 128;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCarter();

builder.Services.AddMediatR(configuration =>
{
    var assembly = typeof(Program).Assembly;
    configuration.RegisterServicesFromAssembly(assembly);
});

builder.Services
    .AddApiDatabase(builder.Configuration)
    .AddApiIdentity(builder.Configuration)
    .AddApiAuth(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://itransition-course-project-ui.netlify.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference("/scalar", options =>
{
    options.WithTitle("API");
    options.WithTheme(ScalarTheme.DeepSpace);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

    options
        .AddPreferredSecuritySchemes(BearerTokenDefaults.AuthenticationScheme)
        .AddAuthorizationCodeFlow(BearerTokenDefaults.AuthenticationScheme,
            flow =>
            {
                flow.Token = "CfDJ8DNoxjbbLI5MpNKLIgUgqrV6R_861DaACEuyX3-trGz2m8uncYDjivuHeBasI-FKiMYRy0uJxWsO4M7O6-jdXDkkGxXIcvP0Z1wV9k_EAW8ejOEn49_HbOkFEGRskVdxW-ZqAUniChMy9_W1VqnLIVu3Jn9zvYhd4MY2ogVhvY83qNgx9tRuaMilaOzAROJ3tgU82DNxWQHcS4bUfJSrvLO6SFiYRD1ipkCTlIIc5B0kJsAyrcY22sOmyko46rr7sD9eMviD5uPGwGgJ6OmfPbGNHsSaciXUmXL7bO7zrLMsib08au_P8yBppa02v8gK6AUXDc57vUOvkuuKRQBTc6sKc1G0uUYVDss4c4mpKoQnDAk3Aw4oHLGGymRRenN2VPk0ZqC-iwEWhb_uHRpNLJrwGLaAF_UVprGu2M4lNpX-3_C263Ac1YctQZW228XsGon3ztw_IJvZTxuU7aJv_yPXOxhaHskbPQ-mC402lIz2jNtRuOD9uQuOS_2B6okn0dcOA8hJasIOcqPAgjqwSnA9FfZqjhhg8X1EE08Px5Y_dLyPoTn26tM7W8Zm1n58EF_nJZx5QmyoNliAiDyj36cRkyLWbYnhX7Oxw8OZPVUZE-NO0ytpYI0gDvgjH2SdBfuUx9EsMOwXFK6_TzRvapk";
            });
}).AllowAnonymous();


app.MapControllers();
app.MapCarter();

app.MapGet("/api/connection-string", (IConfiguration config) =>
    {
        var cs = config.GetConnectionString("Database");
        return Results.Ok(new { connectionString = cs });
    })
    .AllowAnonymous();

app.Run();