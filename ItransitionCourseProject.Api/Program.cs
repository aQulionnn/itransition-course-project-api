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
                flow.Token = "CfDJ8DNoxjbbLI5MpNKLIgUgqrWgwbTOSO2IX95_JSwNjMvtbzB_JCVDhbsbZL6DjWcQg716lJ5ilAMUmXvny7DXkoizX-RKXlAfaIECKFJYZJoBAhiOWyvb6I5ovE2AKTkuyY6vpkrflA-8t6TsqI6TjSJ42YAIP0gQgjfVfrOdzF0OH-U5B4-Be1dR0wZ2mtJVVCL2HHIWwsb640Ha8ZjD-m0ubjaEW3QakRSp3eXbSL8Y3fQITQoGMg_PT6oQHIH_lydI5E7ObNZSJdwYlL-pR2LR7knXSvjre_G_PNeH9cE5F2nD3zuDq_wbDQA6hYGKkGcOy_aVOqGu51fDFVMzZalLtsZrkv-nKfHUe61F7q8uorzJZmyPAT2a-GeMxE_ure9gN1T8IHAKfcvpZS9rPdwDSb8HIhgXStKknz9xB0VxysofjEJB6Loh5PNH_cVVor7CrfP_bulsW_Posr8xoRdX4qM068l2ZH16I4vqdzgKHhRLzTI6WF5PqQKiq8GGzOIQ7hKojsMAz5S3aM-D5ioXCmiar63E4bi1S35vQzxXznKeRwF-ttWbVns1KgclEghIfnJXUzm5coAlj5VBHqQfRGUNd112KtqsLjGmjIJGwUmeHH0gfWz1Qt9UPPt0jRGF0CcaHzJbQ8EYi3EliSw";
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