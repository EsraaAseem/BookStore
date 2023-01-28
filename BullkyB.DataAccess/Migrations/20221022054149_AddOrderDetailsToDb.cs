using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BullkyB.DataAccess.Migrations
{
    public partial class AddOrderDetailsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_Products_ProductId",
                table: "orderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderDetails",
                table: "orderDetails");

            migrationBuilder.RenameTable(
                name: "orderDetails",
                newName: "orderDetailes");

            migrationBuilder.RenameIndex(
                name: "IX_orderDetails_ProductId",
                table: "orderDetailes",
                newName: "IX_orderDetailes_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_orderDetails_OrderId",
                table: "orderDetailes",
                newName: "IX_orderDetailes_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderDetailes",
                table: "orderDetailes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetailes_orderHeaders_OrderId",
                table: "orderDetailes",
                column: "OrderId",
                principalTable: "orderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetailes_Products_ProductId",
                table: "orderDetailes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetailes_orderHeaders_OrderId",
                table: "orderDetailes");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetailes_Products_ProductId",
                table: "orderDetailes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderDetailes",
                table: "orderDetailes");

            migrationBuilder.RenameTable(
                name: "orderDetailes",
                newName: "orderDetails");

            migrationBuilder.RenameIndex(
                name: "IX_orderDetailes_ProductId",
                table: "orderDetails",
                newName: "IX_orderDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_orderDetailes_OrderId",
                table: "orderDetails",
                newName: "IX_orderDetails_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderDetails",
                table: "orderDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderId",
                table: "orderDetails",
                column: "OrderId",
                principalTable: "orderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_Products_ProductId",
                table: "orderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
