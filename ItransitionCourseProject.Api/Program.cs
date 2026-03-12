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
                    "CfDJ8DNoxjbbLI5MpNKLIgUgqrXC_N0xX1w-sxp15GBBhNGZU63WSgzh1xu9Muv2PmfTVv9BMZjY6ksIlkZX68Ezm02AeoGCBflxNfOgr5RVA7cnje9WhXRdMiUM45dRBBpMRA7VCrvqN-2yvHl5lPrb-ZyixpD_q0QlWFLkTuchHW-Fy0HN4A0uLrW5yuj12oZxoT5o_h-oYAE9Aw9mEKQE1pz6OdPErOQePSB4X1EP42hCViHGZJy1KGx-Xcizt82NNmJocUL1G45yOXTXR1bvekvRDbCrthJpm77o7DcXI1OlvH0oLg9u6Jc-6O10GCD7CC0wT_CvjtgdnKGbDZHSE_NjAjjCtr4PdCfsnuqB33UYkBVuQqCNU14X67StgjMF-HLTHKsmhZO3s2Rx-zWqJO4kj_ONG-mCbch-dlHIuzy__dQ69W_xaas_l2bQSSWOwms7TfFFqdfiC1sg7Sqxr5ayvwLMMHVjn_nOf39yTeaii4aBa0_UrwudLsk54XKBomuGR2xFCcEERAVud0qqtDjKBPyLSh_qTNTBx9wan5Nj";
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