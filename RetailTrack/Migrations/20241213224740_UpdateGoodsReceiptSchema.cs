using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGoodsReceiptSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_Materials_MaterialId",
                table: "GoodsReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_ProductSizes_SizeId",
                table: "GoodsReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsReceipts",
                table: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_SizeId",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "GoodsReceipts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiptId",
                table: "GoodsReceipts",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "GoodsReceipts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsReceipts",
                table: "GoodsReceipts",
                column: "ReceiptId");

            migrationBuilder.CreateTable(
                name: "GoodsReceiptDetails",
                columns: table => new
                {
                    ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetailId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptDetails", x => new { x.ReceiptId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_GoodsReceiptDetails_GoodsReceipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "GoodsReceipts",
                        principalColumn: "ReceiptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptDetails_ProductSizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "ProductSizes",
                        principalColumn: "Size_Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_MaterialId",
                table: "GoodsReceipts",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptDetails_MaterialId",
                table: "GoodsReceiptDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptDetails_SizeId",
                table: "GoodsReceiptDetails",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_Materials_MaterialId",
                table: "GoodsReceipts",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_Materials_MaterialId",
                table: "GoodsReceipts");

            migrationBuilder.DropTable(
                name: "GoodsReceiptDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsReceipts",
                table: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_MaterialId",
                table: "GoodsReceipts");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "GoodsReceipts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 0)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiptId",
                table: "GoodsReceipts",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "GoodsReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "GoodsReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "GoodsReceipts",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsReceipts",
                table: "GoodsReceipts",
                columns: new[] { "MaterialId", "ReceiptId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_SizeId",
                table: "GoodsReceipts",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_Materials_MaterialId",
                table: "GoodsReceipts",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_ProductSizes_SizeId",
                table: "GoodsReceipts",
                column: "SizeId",
                principalTable: "ProductSizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
