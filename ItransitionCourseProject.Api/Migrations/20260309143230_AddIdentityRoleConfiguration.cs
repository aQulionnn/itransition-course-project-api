using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItransitionCourseProject.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityRoleConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "79c910a6-6b18-4d3c-8e9e-72eff6731a9d", "15de1459-cf00-44d8-b901-ca198d864aa9", "Admin", "ADMIN" },
                    { "b3e9caf3-84e1-4e11-9734-6aa8e1e1d2cd", "276cbe53-61e6-43d1-9750-1e8de45e6088", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "79c910a6-6b18-4d3c-8e9e-72eff6731a9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "b3e9caf3-84e1-4e11-9734-6aa8e1e1d2cd");
        }
    }
}
