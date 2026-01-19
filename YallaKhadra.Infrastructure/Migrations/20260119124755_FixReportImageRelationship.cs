using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReportImageRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportImages_Reports_WasteReportId",
                table: "ReportImages");

            migrationBuilder.DropIndex(
                name: "IX_ReportImages_WasteReportId",
                table: "ReportImages");

            migrationBuilder.DropColumn(
                name: "WasteReportId",
                table: "ReportImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WasteReportId",
                table: "ReportImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportImages_WasteReportId",
                table: "ReportImages",
                column: "WasteReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportImages_Reports_WasteReportId",
                table: "ReportImages",
                column: "WasteReportId",
                principalTable: "Reports",
                principalColumn: "Id");
        }
    }
}
