using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9780c339-65f4-42fd-80d9-027e072bd570", "82d29c6e-0376-4709-80fe-7d23604055f3", "Administrator", "ADMINISTRATOR" },
                    { "c1f36078-5058-43eb-909d-14dbfbe1182d", "48632ef6-990b-461a-914f-f992af2c9568", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9780c339-65f4-42fd-80d9-027e072bd570");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1f36078-5058-43eb-909d-14dbfbe1182d");
        }
    }
}
