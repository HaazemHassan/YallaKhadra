using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateproudctandproductimageentitesschema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ecommerce");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "ecommerce");

            migrationBuilder.RenameTable(
                name: "ProductImage",
                newName: "ProductImage",
                newSchema: "ecommerce");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Products",
                schema: "ecommerce",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "ProductImage",
                schema: "ecommerce",
                newName: "ProductImage");
        }
    }
}
