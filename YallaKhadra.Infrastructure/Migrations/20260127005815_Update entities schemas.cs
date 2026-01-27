using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateentitiesschemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "green");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "points");

            migrationBuilder.RenameTable(
                name: "WasteScanImages",
                newName: "WasteScanImages",
                newSchema: "green");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Reports",
                newSchema: "green");

            migrationBuilder.RenameTable(
                name: "ReportImages",
                newName: "ReportImages",
                newSchema: "green");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshToken",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PointsTransactions",
                newName: "PointsTransactions",
                newSchema: "points");

            migrationBuilder.RenameTable(
                name: "CleanupTasks",
                newName: "CleanupTasks",
                newSchema: "green");

            migrationBuilder.RenameTable(
                name: "CleanupImages",
                newName: "CleanupImages",
                newSchema: "green");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AIWasteScans",
                newName: "AIWasteScans",
                newSchema: "green");

            migrationBuilder.AlterColumn<int>(
                name: "PointsBalance",
                schema: "identity",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "WasteScanImages",
                schema: "green",
                newName: "WasteScanImages");

            migrationBuilder.RenameTable(
                name: "Reports",
                schema: "green",
                newName: "Reports");

            migrationBuilder.RenameTable(
                name: "ReportImages",
                schema: "green",
                newName: "ReportImages");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                schema: "identity",
                newName: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "PointsTransactions",
                schema: "points",
                newName: "PointsTransactions");

            migrationBuilder.RenameTable(
                name: "CleanupTasks",
                schema: "green",
                newName: "CleanupTasks");

            migrationBuilder.RenameTable(
                name: "CleanupImages",
                schema: "green",
                newName: "CleanupImages");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "identity",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "identity",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "identity",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "identity",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "identity",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "identity",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "identity",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AIWasteScans",
                schema: "green",
                newName: "AIWasteScans");

            migrationBuilder.AlterColumn<int>(
                name: "PointsBalance",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
