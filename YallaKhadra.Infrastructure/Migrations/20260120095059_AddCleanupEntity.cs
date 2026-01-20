using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCleanupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleanupTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalWasteType = table.Column<int>(type: "int", nullable: false),
                    FinalWeightInKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    WorkerId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleanupTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleanupTasks_AspNetUsers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleanupTasks_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CleanupImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CleanupTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleanupImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleanupImages_CleanupTasks_CleanupTaskId",
                        column: x => x.CleanupTaskId,
                        principalTable: "CleanupTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleanupImages_Images_Id",
                        column: x => x.Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleanupImages_CleanupTaskId",
                table: "CleanupImages",
                column: "CleanupTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CleanupTasks_ReportId",
                table: "CleanupTasks",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CleanupTasks_WorkerId",
                table: "CleanupTasks",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleanupImages");

            migrationBuilder.DropTable(
                name: "CleanupTasks");
        }
    }
}
