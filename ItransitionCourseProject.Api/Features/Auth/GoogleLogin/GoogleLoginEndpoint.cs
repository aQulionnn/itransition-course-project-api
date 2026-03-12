using System.Security.Claims;
using Carter;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace ItransitionCourseProject.Api.Features.Auth.GoogleLogin;

public class GoogleLoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/google-login", async ([FromQuery] string returnUrl, LinkGenerator linkGenerator,
            SignInManager<AppUser> signInManager, HttpContext context, IFeatureManager featureManager) =>
        {
            if (!await featureManager.IsEnabledAsync("EnableGoogleOAuth"))
                return Results.NotFound();
            
            var props = signInManager.ConfigureExternalAuthenticationProperties("Google",
                linkGenerator.GetPathByName(context, "GoogleLoginCallback") + $"?returnUrl={returnUrl}");

            return Results.Challenge(props, ["Google"]);
        }).AllowAnonymous();

        app.MapGet("/api/auth/google-callback", async ([FromQuery] string returnUrl,
                HttpContext context,
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager) =>
            {
                var result = await context.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!result.Succeeded)
                    return Results.Unauthorized();

                var email = result.Principal.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(email))
                    return Results.BadRequest("Google did not return email.");

                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = email,
                        Email = email
                    };

                    await userManager.CreateAsync(user);
                    await userManager.AddToRoleAsync(user, "User");
                }

                var info = new UserLoginInfo("Google",
                    result.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty, "Google");

                await userManager.AddLoginAsync(user, info);
                await signInManager.SignInAsync(user, true);

                return Results.Redirect(returnUrl);
            })
            .WithName("GoogleLoginCallback")
            .AllowAnonymous();
    }
}