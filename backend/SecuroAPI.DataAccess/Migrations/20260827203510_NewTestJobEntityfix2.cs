using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewTestJobEntityfix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TestJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TestJobs");
        }
    }
}
