using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MaterialSizes_MaterialId_SizeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MaterialId_SizeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Movements_ProductId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ProductStocks",
                newName: "Stock");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "ProductStocks",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomPrice",
                table: "ProductStocks",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "ProductStocks",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "GeneralPrice",
                table: "Products",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_MaterialId_SizeId",
                table: "ProductStocks",
                columns: new[] { "MaterialId", "SizeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_MaterialSizes_MaterialId_SizeId",
                table: "ProductStocks",
                columns: new[] { "MaterialId", "SizeId" },
                principalTable: "MaterialSizes",
                principalColumns: new[] { "MaterialId", "SizeId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_MaterialSizes_MaterialId_SizeId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_MaterialId_SizeId",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "CustomPrice",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "GeneralPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "ProductStocks",
                newName: "Quantity");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "Products",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Movements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaterialId_SizeId",
                table: "Products",
                columns: new[] { "MaterialId", "SizeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductId",
                table: "Movements",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MaterialSizes_MaterialId_SizeId",
                table: "Products",
                columns: new[] { "MaterialId", "SizeId" },
                principalTable: "MaterialSizes",
                principalColumns: new[] { "MaterialId", "SizeId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
