using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddproductandProductImageentityproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "ecommerce",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "ecommerce",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "ecommerce",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "ecommerce",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PointsCost",
                schema: "ecommerce",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                schema: "ecommerce",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                schema: "ecommerce",
                table: "ProductImage",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PointsCost",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                schema: "ecommerce",
                table: "Products");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                schema: "ecommerce",
                table: "ProductImage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
