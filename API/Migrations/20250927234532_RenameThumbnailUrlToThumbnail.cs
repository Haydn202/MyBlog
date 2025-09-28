using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RenameThumbnailUrlToThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26932516-3a19-42d3-8097-5931e8fcfaf5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b865dbcf-1016-4c6c-a48f-4d4fc9a1c304");

            migrationBuilder.RenameColumn(
                name: "ThumbnailUrl",
                table: "Posts",
                newName: "Thumbnail");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "35db37c3-12cb-4f3a-8c0c-7dc11866f25f", null, "Admin", "ADMIN" },
                    { "7e7a815b-a2c0-4736-9211-bf598af5aae2", null, "None", "NONE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35db37c3-12cb-4f3a-8c0c-7dc11866f25f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e7a815b-a2c0-4736-9211-bf598af5aae2");

            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "Posts",
                newName: "ThumbnailUrl");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "26932516-3a19-42d3-8097-5931e8fcfaf5", null, "Admin", "ADMIN" },
                    { "b865dbcf-1016-4c6c-a48f-4d4fc9a1c304", null, "None", "NONE" }
                });
        }
    }
}
