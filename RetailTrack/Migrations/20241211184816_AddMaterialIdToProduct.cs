using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Products_ProductId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_ProductId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Materials");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "Products",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Materials",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ProductId",
                table: "Materials",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Products_ProductId",
                table: "Materials",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
