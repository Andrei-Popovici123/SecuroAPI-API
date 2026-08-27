using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuroAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewTestJobEntityfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestJobs_APIRegistry_ApiRegistryAPIID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_APIID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_ApiRegistryAPIID",
                table: "TestJobs");

            migrationBuilder.DropColumn(
                name: "ApiRegistryAPIID",
                table: "TestJobs");

            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_APIID",
                table: "TestJobs",
                column: "APIID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestJobs_APIRegistry_APIID",
                table: "TestJobs",
                column: "APIID",
                principalTable: "APIRegistry",
                principalColumn: "APIID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestJobs_APIRegistry_APIID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_APIID",
                table: "TestJobs");

            migrationBuilder.AddColumn<Guid>(
                name: "ApiRegistryAPIID",
                table: "TestJobs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_TestJobs_APIRegistry_ApiRegistryAPIID",
                table: "TestJobs",
                column: "ApiRegistryAPIID",
                principalTable: "APIRegistry",
                principalColumn: "APIID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
