using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewTestJobEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestJobs",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApiRegistryAPIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestJobs", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_TestJobs_APIRegistry_ApiRegistryAPIID",
                        column: x => x.ApiRegistryAPIID,
                        principalTable: "APIRegistry",
                        principalColumn: "APIID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_APIID",
                table: "TestJobs",
                column: "APIID",
                unique: true,
                filter: "[Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_ApiRegistryAPIID",
                table: "TestJobs",
                column: "ApiRegistryAPIID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestJobs");
        }
    }
}
