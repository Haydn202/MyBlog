using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddPostStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25a0ad5c-98e0-411e-bd42-4ab2fe7f6812");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bb12e4f-9b3f-44d5-9f6e-fee4613aece5");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13447335-c3f8-41e2-b8b5-b3a2e3b693e4", null, "None", "NONE" },
                    { "53e6e897-61cf-4a08-9205-eed2233021fb", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13447335-c3f8-41e2-b8b5-b3a2e3b693e4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53e6e897-61cf-4a08-9205-eed2233021fb");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25a0ad5c-98e0-411e-bd42-4ab2fe7f6812", null, "None", "NONE" },
                    { "3bb12e4f-9b3f-44d5-9f6e-fee4613aece5", null, "Admin", "ADMIN" }
                });
        }
    }
}
