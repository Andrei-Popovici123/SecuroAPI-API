using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedEntitities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AnomalyLogs",
                columns: table => new
                {
                    AnomalyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnomalyType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnomalyLogs", x => x.AnomalyId);
                    table.ForeignKey(
                        name: "FK_AnomalyLogs_APIRegistry_APIID",
                        column: x => x.APIID,
                        principalTable: "APIRegistry",
                        principalColumn: "APIID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VulnerabilityScore = table.Column<int>(type: "int", nullable: false),
                    NumberOfTests = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<int>(type: "int", nullable: false),
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_APIRegistry_APIID",
                        column: x => x.APIID,
                        principalTable: "APIRegistry",
                        principalColumn: "APIID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestConfigs",
                columns: table => new
                {
                    ConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnabledTestIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConfigs", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_TestConfigs_APIRegistry_APIID",
                        column: x => x.APIID,
                        principalTable: "APIRegistry",
                        principalColumn: "APIID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreReport",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Low"),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_ScoreReport_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyLogs_APIID",
                table: "AnomalyLogs",
                column: "APIID");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_APIID",
                table: "Ratings",
                column: "APIID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreReport_RatingId",
                table: "ScoreReport",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_TestConfigs_APIID",
                table: "TestConfigs",
                column: "APIID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnomalyLogs");

            migrationBuilder.DropTable(
                name: "ScoreReport");

            migrationBuilder.DropTable(
                name: "TestConfigs");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "AspNetUsers");
        }
    }
}
