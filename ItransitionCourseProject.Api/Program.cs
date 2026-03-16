using Azure.Identity;
using Carter;
using DotNetEnv;
using ItransitionCourseProject.Api.Extensions;
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
                flow.Token = "";
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