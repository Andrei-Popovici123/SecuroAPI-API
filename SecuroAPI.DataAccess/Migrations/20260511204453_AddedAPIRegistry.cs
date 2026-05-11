using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedAPIRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIRegistry",
                columns: table => new
                {
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    TargetURL = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    AuthType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Inactive"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIRegistry", x => x.APIID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APIRegistry_APIID",
                table: "APIRegistry",
                column: "APIID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIRegistry");
        }
    }
}
