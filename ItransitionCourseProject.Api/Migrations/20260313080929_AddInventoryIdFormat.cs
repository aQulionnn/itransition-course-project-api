using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItransitionCourseProject.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryIdFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "2249781f-facc-4fb9-9d37-9835a0d64bcd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "52d04c6b-787a-4bb5-ad0e-c8880f04a140");

            migrationBuilder.CreateTable(
                name: "inventory_id_formats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_id_formats", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_id_formats_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_id_elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: true),
                    padding = table.Column<int>(type: "integer", nullable: false),
                    date_format = table.Column<string>(type: "text", nullable: true),
                    format_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_id_elements", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_id_elements_inventory_id_formats_format_id",
                        column: x => x.format_id,
                        principalTable: "inventory_id_formats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "2cf3a39e-2840-4cf8-b566-573d68523ada", "abe8513e-22d0-4e12-a838-21b8644017bc", "User", "USER" },
                    { "f9abaff4-c4dc-477c-af41-2549154e50e6", "e3e75d77-8213-4ba8-89ed-188432d3d2d0", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_id_elements_format_id",
                table: "inventory_id_elements",
                column: "format_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_id_formats_inventory_id",
                table: "inventory_id_formats",
                column: "inventory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_id_elements");

            migrationBuilder.DropTable(
                name: "inventory_id_formats");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "2cf3a39e-2840-4cf8-b566-573d68523ada");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "f9abaff4-c4dc-477c-af41-2549154e50e6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "2249781f-facc-4fb9-9d37-9835a0d64bcd", "fa540dcc-9b89-48cc-9d65-5a54202c6917", "User", "USER" },
                    { "52d04c6b-787a-4bb5-ad0e-c8880f04a140", "b9a95224-bb6b-4e02-bbef-e092542799be", "Admin", "ADMIN" }
                });
        }
    }
}
