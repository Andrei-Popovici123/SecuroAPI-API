using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoredEndpoints",
                columns: table => new
                {
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    APIID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IntervalSeconds = table.Column<int>(type: "int", nullable: false),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastOk = table.Column<bool>(type: "bit", nullable: true),
                    LastStatusCode = table.Column<int>(type: "int", nullable: true),
                    LastLatencyMs = table.Column<int>(type: "int", nullable: true),
                    LastErrorType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasHsts = table.Column<bool>(type: "bit", nullable: false),
                    HasCsp = table.Column<bool>(type: "bit", nullable: false),
                    HasNosniff = table.Column<bool>(type: "bit", nullable: false),
                    HasFrameOptions = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoredEndpoints", x => x.EndpointId);
                    table.CheckConstraint("CK_MonitoredEndpoint_Interval", "[IntervalSeconds] >= 60");
                    table.ForeignKey(
                        name: "FK_MonitoredEndpoints_APIRegistry_APIID",
                        column: x => x.APIID,
                        principalTable: "APIRegistry",
                        principalColumn: "APIID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ok = table.Column<bool>(type: "bit", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    LatencyMs = table.Column<int>(type: "int", nullable: false),
                    ErrorType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryPoints_MonitoredEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "MonitoredEndpoints",
                        principalColumn: "EndpointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoredEndpoints_APIID_Url",
                table: "MonitoredEndpoints",
                columns: new[] { "APIID", "Url" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoredEndpoints_IsActive_LastCheckedAt",
                table: "MonitoredEndpoints",
                columns: new[] { "IsActive", "LastCheckedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryPoints_EndpointId_CheckedAt",
                table: "TelemetryPoints",
                columns: new[] { "EndpointId", "CheckedAt" })
                .Annotation("SqlServer:Include", new[] { "Ok", "LatencyMs", "StatusCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetryPoints");

            migrationBuilder.DropTable(
                name: "MonitoredEndpoints");
        }
    }
}
