using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixbug1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_AspNetUsers_UserId1",
                table: "photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReviewedById1",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UserId1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReviewedById1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_UserId1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_photos_UserId1",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "ReviewedById1",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "photos");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReviewedById",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "photos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReviewedById",
                table: "Reports",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_photos_UserId",
                table: "photos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_AspNetUsers_UserId",
                table: "photos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReviewedById",
                table: "Reports",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_AspNetUsers_UserId",
                table: "photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReviewedById",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReviewedById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_UserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_photos_UserId",
                table: "photos");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewedById",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedById1",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "photos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "photos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReviewedById1",
                table: "Reports",
                column: "ReviewedById1");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId1",
                table: "Reports",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_photos_UserId1",
                table: "photos",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_AspNetUsers_UserId1",
                table: "photos",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReviewedById1",
                table: "Reports",
                column: "ReviewedById1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UserId1",
                table: "Reports",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
