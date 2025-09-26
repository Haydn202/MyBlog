using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddColorEnumToTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13447335-c3f8-41e2-b8b5-b3a2e3b693e4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53e6e897-61cf-4a08-9205-eed2233021fb");

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Topics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "26932516-3a19-42d3-8097-5931e8fcfaf5", null, "Admin", "ADMIN" },
                    { "b865dbcf-1016-4c6c-a48f-4d4fc9a1c304", null, "None", "NONE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26932516-3a19-42d3-8097-5931e8fcfaf5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b865dbcf-1016-4c6c-a48f-4d4fc9a1c304");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Topics");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13447335-c3f8-41e2-b8b5-b3a2e3b693e4", null, "None", "NONE" },
                    { "53e6e897-61cf-4a08-9205-eed2233021fb", null, "Admin", "ADMIN" }
                });
        }
    }
}
