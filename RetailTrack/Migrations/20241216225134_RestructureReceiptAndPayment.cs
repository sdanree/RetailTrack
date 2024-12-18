using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class RestructureReceiptAndPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Products_ProductSizes_ProductSizeId",
            //     table: "Products");

            // migrationBuilder.DropTable(
            //     name: "GoodsReceiptDetails");

            // migrationBuilder.DropTable(
            //     name: "GoodsReceipts");

            // migrationBuilder.DropTable(
            //     name: "ProductSizes");

            // migrationBuilder.DropIndex(
            //     name: "IX_Products_ProductSizeId",
            //     table: "Products");

            // migrationBuilder.DropColumn(
            //     name: "ProductSizeId",
            //     table: "Products");

            // migrationBuilder.RenameColumn(
            //     name: "PaymentMethod_Name",
            //     table: "PaymentMethods",
            //     newName: "Name");

            // migrationBuilder.RenameColumn(
            //     name: "PaymentMethod_Id",
            //     table: "PaymentMethods",
            //     newName: "PaymentMethodId");

            // migrationBuilder.AddColumn<int>(
            //     name: "SizeId",
            //     table: "Materials",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.CreateTable(
            //     name: "Provider",
            //     columns: table => new
            //     {
            //         Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         BusinessName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         RUT = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         Address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Provider", x => x.Id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "Sizes",
            //     columns: table => new
            //     {
            //         Size_Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //         Size_Name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Sizes", x => x.Size_Id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "Receipts",
            //     columns: table => new
            //     {
            //         ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         ReceiptAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //         ReceiptDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         ProviderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Receipts", x => x.ReceiptId);
            //         table.ForeignKey(
            //             name: "FK_Receipts_Provider_ProviderId",
            //             column: x => x.ProviderId,
            //             principalTable: "Provider",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "ReceiptDetails",
            //     columns: table => new
            //     {
            //         ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         DetailId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         Quantity = table.Column<int>(type: "int", nullable: false),
            //         UnitCost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
            //         SizeId = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_ReceiptDetails", x => new { x.ReceiptId, x.MaterialId });
            //         table.ForeignKey(
            //             name: "FK_ReceiptDetails_Materials_MaterialId",
            //             column: x => x.MaterialId,
            //             principalTable: "Materials",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Restrict);
            //         table.ForeignKey(
            //             name: "FK_ReceiptDetails_Receipts_ReceiptId",
            //             column: x => x.ReceiptId,
            //             principalTable: "Receipts",
            //             principalColumn: "ReceiptId",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_ReceiptDetails_Sizes_SizeId",
            //             column: x => x.SizeId,
            //             principalTable: "Sizes",
            //             principalColumn: "Size_Id",
            //             onDelete: ReferentialAction.Restrict);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "ReceiptPayments",
            //     columns: table => new
            //     {
            //         ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         PaymentMethodId = table.Column<int>(type: "int", nullable: false),
            //         ReceiptPaymentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         Percentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_ReceiptPayments", x => new { x.ReceiptId, x.PaymentMethodId });
            //         table.ForeignKey(
            //             name: "FK_ReceiptPayments_PaymentMethods_PaymentMethodId",
            //             column: x => x.PaymentMethodId,
            //             principalTable: "PaymentMethods",
            //             principalColumn: "PaymentMethodId",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_ReceiptPayments_Receipts_ReceiptId",
            //             column: x => x.ReceiptId,
            //             principalTable: "Receipts",
            //             principalColumn: "ReceiptId",
            //             onDelete: ReferentialAction.Cascade);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Materials_SizeId",
            //     table: "Materials",
            //     column: "SizeId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_ReceiptDetails_MaterialId",
            //     table: "ReceiptDetails",
            //     column: "MaterialId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_ReceiptDetails_SizeId",
            //     table: "ReceiptDetails",
            //     column: "SizeId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_ReceiptPayments_PaymentMethodId",
            //     table: "ReceiptPayments",
            //     column: "PaymentMethodId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Receipts_ProviderId",
            //     table: "Receipts",
            //     column: "ProviderId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Materials_Sizes_SizeId",
            //     table: "Materials",
            //     column: "SizeId",
            //     principalTable: "Sizes",
            //     principalColumn: "Size_Id",
            //     onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Materials_Sizes_SizeId",
            //     table: "Materials");

            // migrationBuilder.DropTable(
            //     name: "ReceiptDetails");

            // migrationBuilder.DropTable(
            //     name: "ReceiptPayments");

            // migrationBuilder.DropTable(
            //     name: "Sizes");

            // migrationBuilder.DropTable(
            //     name: "Receipts");

            // migrationBuilder.DropTable(
            //     name: "Provider");

            // migrationBuilder.DropIndex(
            //     name: "IX_Materials_SizeId",
            //     table: "Materials");

            // migrationBuilder.DropColumn(
            //     name: "SizeId",
            //     table: "Materials");

            // migrationBuilder.RenameColumn(
            //     name: "Name",
            //     table: "PaymentMethods",
            //     newName: "PaymentMethod_Name");

            // migrationBuilder.RenameColumn(
            //     name: "PaymentMethodId",
            //     table: "PaymentMethods",
            //     newName: "PaymentMethod_Id");

            // migrationBuilder.AddColumn<int>(
            //     name: "ProductSizeId",
            //     table: "Products",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.CreateTable(
            //     name: "GoodsReceipts",
            //     columns: table => new
            //     {
            //         ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         PaymentMethodId = table.Column<int>(type: "int", nullable: false),
            //         MaterialId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
            //         ReceiptDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_GoodsReceipts", x => x.ReceiptId);
            //         table.ForeignKey(
            //             name: "FK_GoodsReceipts_Materials_MaterialId",
            //             column: x => x.MaterialId,
            //             principalTable: "Materials",
            //             principalColumn: "Id");
            //         table.ForeignKey(
            //             name: "FK_GoodsReceipts_PaymentMethods_PaymentMethodId",
            //             column: x => x.PaymentMethodId,
            //             principalTable: "PaymentMethods",
            //             principalColumn: "PaymentMethod_Id",
            //             onDelete: ReferentialAction.Cascade);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "ProductSizes",
            //     columns: table => new
            //     {
            //         Size_Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //         Size_Name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_ProductSizes", x => x.Size_Id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "GoodsReceiptDetails",
            //     columns: table => new
            //     {
            //         ReceiptId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         SizeId = table.Column<int>(type: "int", nullable: false),
            //         DetailId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //         Quantity = table.Column<int>(type: "int", nullable: false),
            //         UnitCost = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_GoodsReceiptDetails", x => new { x.ReceiptId, x.MaterialId });
            //         table.ForeignKey(
            //             name: "FK_GoodsReceiptDetails_GoodsReceipts_ReceiptId",
            //             column: x => x.ReceiptId,
            //             principalTable: "GoodsReceipts",
            //             principalColumn: "ReceiptId",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_GoodsReceiptDetails_Materials_MaterialId",
            //             column: x => x.MaterialId,
            //             principalTable: "Materials",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Restrict);
            //         table.ForeignKey(
            //             name: "FK_GoodsReceiptDetails_ProductSizes_SizeId",
            //             column: x => x.SizeId,
            //             principalTable: "ProductSizes",
            //             principalColumn: "Size_Id",
            //             onDelete: ReferentialAction.Restrict);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Products_ProductSizeId",
            //     table: "Products",
            //     column: "ProductSizeId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_GoodsReceiptDetails_MaterialId",
            //     table: "GoodsReceiptDetails",
            //     column: "MaterialId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_GoodsReceiptDetails_SizeId",
            //     table: "GoodsReceiptDetails",
            //     column: "SizeId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_GoodsReceipts_MaterialId",
            //     table: "GoodsReceipts",
            //     column: "MaterialId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_GoodsReceipts_PaymentMethodId",
            //     table: "GoodsReceipts",
            //     column: "PaymentMethodId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Products_ProductSizes_ProductSizeId",
            //     table: "Products",
            //     column: "ProductSizeId",
            //     principalTable: "ProductSizes",
            //     principalColumn: "Size_Id",
            //     onDelete: ReferentialAction.Cascade);
        }
    }
}
