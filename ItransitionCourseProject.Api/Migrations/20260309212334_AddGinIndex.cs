using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItransitionCourseProject.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGinIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "79c910a6-6b18-4d3c-8e9e-72eff6731a9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "b3e9caf3-84e1-4e11-9734-6aa8e1e1d2cd");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "22cc2157-8c97-401e-869e-238b1de6aa0d", "1e5c25fa-06ae-40da-9905-a686e6f5ec63", "Admin", "ADMIN" },
                    { "bc879531-efd1-40ab-ac41-4475f2808c29", "2b2b6046-f3e8-4d7e-a870-8eb61b313e77", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_posts_content",
                table: "posts",
                column: "content")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "ix_item_field_values_value",
                table: "item_field_values",
                column: "value")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "ix_inventories_title_description",
                table: "inventories",
                columns: new[] { "title", "description" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_posts_content",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "ix_item_field_values_value",
                table: "item_field_values");

            migrationBuilder.DropIndex(
                name: "ix_inventories_title_description",
                table: "inventories");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "22cc2157-8c97-401e-869e-238b1de6aa0d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "bc879531-efd1-40ab-ac41-4475f2808c29");

            migrationBuilder.DropColumn(
                name: "is_blocked",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "79c910a6-6b18-4d3c-8e9e-72eff6731a9d", "15de1459-cf00-44d8-b901-ca198d864aa9", "Admin", "ADMIN" },
                    { "b3e9caf3-84e1-4e11-9734-6aa8e1e1d2cd", "276cbe53-61e6-43d1-9750-1e8de45e6088", "User", "USER" }
                });
        }
    }
}
