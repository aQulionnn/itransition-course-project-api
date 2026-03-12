using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ItransitionCourseProject.Api.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddApiAuth(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("FeatureManagement:EnableGoogleOAuth"))
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                });
        }
        else
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
                })
                .AddBearerToken();
        }


        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}