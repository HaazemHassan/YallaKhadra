using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIWasteScanEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIWasteScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AIPredictedType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AIIsRecyclable = table.Column<bool>(type: "bit", nullable: false),
                    AIExplanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIWasteScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIWasteScans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WasteScanImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AIWasteScanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteScanImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WasteScanImages_AIWasteScans_AIWasteScanId",
                        column: x => x.AIWasteScanId,
                        principalTable: "AIWasteScans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WasteScanImages_Images_Id",
                        column: x => x.Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIWasteScans_UserId",
                table: "AIWasteScans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WasteScanImages_AIWasteScanId",
                table: "WasteScanImages",
                column: "AIWasteScanId",
                unique: true,
                filter: "[AIWasteScanId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WasteScanImages");

            migrationBuilder.DropTable(
                name: "AIWasteScans");
        }
    }
}
