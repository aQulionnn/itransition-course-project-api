using ItransitionCourseProject.Api.Data;
using ItransitionCourseProject.Api.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddApiDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
            options.UseSnakeCaseNamingConvention();
            options.AddInterceptors(new UpdateAuditableEntitiesInterceptor());
        });

        return services;
    }
}