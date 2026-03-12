using Azure.Identity;
using Carter;
using DotNetEnv;
using ItransitionCourseProject.Api.Extensions;
using Microsoft.AspNetCore.Authentication.BearerToken;
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
                flow.Token =
                    "CfDJ8DNoxjbbLI5MpNKLIgUgqrXeGTNxkDi6JWAJEncSBtyJqyXtqaawfcU7_jJX1-PMCNCCyHbIfwxevYcQ1gOA6sdSyMycYjytto5kM-qQCKWRNjWr4hG6udvk7nMnNU3k1sycIe8Ik1vKv4JwItukxZ7QhJ-U9GezAziHxPZDcHao5y_XJHGsITcZQBB09-cFha-rKt45gK_Uu7wTv-Y_46nhr5xtuVzeWGVlkLwDCqt8qO47FIBPEmczTYKfViJj31gijj6DofaJwkU3lEnWnoeceDWr23D2qfUV8qLLIYHbyJ5XeZiXDgf4lJcHe8aouPwlcQqW73YgfbWE9zKuadsh41U5GoFib0hK1MUHUw2kl37GOuG0bFR3WJhe0sYfFbMDhHEcM2oEPwakXE4wi7Qe7Br6EBFGQHS4NOmYzuFHcKuU1fsqiiPkn7vs3y71OH2fL7liMqN_tM17lBfr9IquYTVLfGSXmDgY7wmJDAhvFFCi3GciznOgyrY88jzm_56YVR8Kykv9_Mr5Ql3AStP1jwvWasGn2OnXwsMTLrTFDIPJo1IifEvngnmIdOKEvw2W7xrq8-bSMY9FWAoK4Omf1OITHA11LLsyXqJLRgRWpASxiOq-bISdyOuDpKBTzzID6pI_FDtr4BDnBNY3SdfSsTMP856L8UyR9aQ9X_OmgF0-eyCjmqO0IFUOPGEekpXnMGfvgriW-zAnMZHK0XQ_6XnIvbns65wQseqIFIeF";
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