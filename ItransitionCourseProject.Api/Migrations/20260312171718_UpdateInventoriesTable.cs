using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItransitionCourseProject.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInventoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "22cc2157-8c97-401e-869e-238b1de6aa0d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "bc879531-efd1-40ab-ac41-4475f2808c29");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                table: "inventories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_on_utc",
                table: "inventories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "2249781f-facc-4fb9-9d37-9835a0d64bcd", "fa540dcc-9b89-48cc-9d65-5a54202c6917", "User", "USER" },
                    { "52d04c6b-787a-4bb5-ad0e-c8880f04a140", "b9a95224-bb6b-4e02-bbef-e092542799be", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "2249781f-facc-4fb9-9d37-9835a0d64bcd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "52d04c6b-787a-4bb5-ad0e-c8880f04a140");

            migrationBuilder.DropColumn(
                name: "created_on_utc",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "modified_on_utc",
                table: "inventories");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "22cc2157-8c97-401e-869e-238b1de6aa0d", "1e5c25fa-06ae-40da-9905-a686e6f5ec63", "Admin", "ADMIN" },
                    { "bc879531-efd1-40ab-ac41-4475f2808c29", "2b2b6046-f3e8-4d7e-a870-8eb61b313e77", "User", "USER" }
                });
        }
    }
}
