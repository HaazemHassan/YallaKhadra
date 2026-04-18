using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YallaKhadra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removeorderidfromshippingdetailsentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderShippingDetails_Orders_OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderShippingDetails_OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingDetailsId",
                schema: "ecommerce",
                table: "Orders",
                column: "ShippingDetailsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderShippingDetails_ShippingDetailsId",
                schema: "ecommerce",
                table: "Orders",
                column: "ShippingDetailsId",
                principalSchema: "ecommerce",
                principalTable: "OrderShippingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderShippingDetails_ShippingDetailsId",
                schema: "ecommerce",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingDetailsId",
                schema: "ecommerce",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderShippingDetails_OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderShippingDetails_Orders_OrderId",
                schema: "ecommerce",
                table: "OrderShippingDetails",
                column: "OrderId",
                principalSchema: "ecommerce",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
