using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addimagetotheuserentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointsTransactions_AspNetUsers_UserId",
                schema: "points",
                table: "PointsTransactions");

            migrationBuilder.AddColumn<int>(
                name: "ProfileImageId",
                schema: "identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserProfileImages",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfileImages_Images_Id",
                        column: x => x.Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileImageId",
                schema: "identity",
                table: "AspNetUsers",
                column: "ProfileImageId",
                unique: true,
                filter: "[ProfileImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserProfileImages_ProfileImageId",
                schema: "identity",
                table: "AspNetUsers",
                column: "ProfileImageId",
                principalSchema: "identity",
                principalTable: "UserProfileImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PointsTransactions_AspNetUsers_UserId",
                schema: "points",
                table: "PointsTransactions",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserProfileImages_ProfileImageId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PointsTransactions_AspNetUsers_UserId",
                schema: "points",
                table: "PointsTransactions");

            migrationBuilder.DropTable(
                name: "UserProfileImages",
                schema: "identity");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileImageId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_PointsTransactions_AspNetUsers_UserId",
                schema: "points",
                table: "PointsTransactions",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
