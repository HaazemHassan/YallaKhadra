using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddphotoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CleanedPhotoUrl",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Reports");

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_photos_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_photos_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_photos_ReportId",
                table: "photos",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_photos_UserId1",
                table: "photos",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.AddColumn<string>(
                name: "CleanedPhotoUrl",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
