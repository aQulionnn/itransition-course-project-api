using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddApiIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                if (!configuration.GetValue<bool>("FeatureManagement:EnableGoogleOAuth"))
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;
                }
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}