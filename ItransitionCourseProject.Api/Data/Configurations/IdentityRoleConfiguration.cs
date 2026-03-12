using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItransitionCourseProject.Api.Data.Configurations;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        List<IdentityRole> roles =
        [
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },

            new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER"
            }
        ];

        builder.HasData(roles);
    }
}