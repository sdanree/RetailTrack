using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class FixProductForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Materials_MaterialId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_Providers_ProviderId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetail_Materials_MaterialId",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetail_PurchaseOrder_PurchaseOrderId",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetail_Sizes_SizeId",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderReceipt_PurchaseOrder_PurchaseOrdersPurchaseOrd~",
                table: "PurchaseOrderReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_Sizes_SizeId",
                table: "ReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_Products_MaterialId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrderDetail",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrder",
                table: "PurchaseOrder");

            migrationBuilder.RenameTable(
                name: "PurchaseOrderDetail",
                newName: "PurchaseOrderDetails");

            migrationBuilder.RenameTable(
                name: "PurchaseOrder",
                newName: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "QuantityRequested",
                table: "Products",
                newName: "SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetail_SizeId",
                table: "PurchaseOrderDetails",
                newName: "IX_PurchaseOrderDetails_SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetail_PurchaseOrderId",
                table: "PurchaseOrderDetails",
                newName: "IX_PurchaseOrderDetails_PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetail_MaterialId",
                table: "PurchaseOrderDetails",
                newName: "IX_PurchaseOrderDetails_MaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrder_ProviderId",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_ProviderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrderDetails",
                table: "PurchaseOrderDetails",
                column: "DetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrders",
                table: "PurchaseOrders",
                column: "PurchaseOrderId");

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductStocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                       // .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerPhone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerAddress = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerRut = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comments = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstimatedCompletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    OrderPaymentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayments", x => x.OrderPaymentId);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "En Proceso" },
                    { 3, "Pronto Para Entrega" },
                    { 4, "Finalizado" }
                });

            migrationBuilder.InsertData(
                table: "ProductStatuses",
                columns: new[] { "Status_Id", "Status_Name" },
                values: new object[,]
                {
                    { 1, "En Producción" },
                    { 2, "Listo Para Venta" },
                    { 3, "Vendido" },
                    { 4, "Devuelto" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaterialId_SizeId",
                table: "Products",
                columns: new[] { "MaterialId", "SizeId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_OrderId",
                table: "OrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_PaymentMethodId",
                table: "OrderPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId",
                table: "ProductStocks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MaterialSizes_MaterialId_SizeId",
                table: "Products",
                columns: new[] { "MaterialId", "SizeId" },
                principalTable: "MaterialSizes",
                principalColumns: new[] { "MaterialId", "SizeId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products",
                column: "ProductStatusId",
                principalTable: "ProductStatuses",
                principalColumn: "Status_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_Materials_MaterialId",
                table: "PurchaseOrderDetails",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_Sizes_SizeId",
                table: "PurchaseOrderDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderReceipt_PurchaseOrders_PurchaseOrdersPurchaseOr~",
                table: "PurchaseOrderReceipt",
                column: "PurchaseOrdersPurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Providers_ProviderId",
                table: "PurchaseOrders",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_Sizes_SizeId",
                table: "ReceiptDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MaterialSizes_MaterialId_SizeId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_Materials_MaterialId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_Sizes_SizeId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderReceipt_PurchaseOrders_PurchaseOrdersPurchaseOr~",
                table: "PurchaseOrderReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Providers_ProviderId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_Sizes_SizeId",
                table: "ReceiptDetails");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderPayments");

            migrationBuilder.DropTable(
                name: "ProductStocks");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Products_MaterialId_SizeId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrders",
                table: "PurchaseOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrderDetails",
                table: "PurchaseOrderDetails");

            migrationBuilder.DeleteData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 4);

            migrationBuilder.RenameTable(
                name: "PurchaseOrders",
                newName: "PurchaseOrder");

            migrationBuilder.RenameTable(
                name: "PurchaseOrderDetails",
                newName: "PurchaseOrderDetail");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "Products",
                newName: "QuantityRequested");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_ProviderId",
                table: "PurchaseOrder",
                newName: "IX_PurchaseOrder_ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetails_SizeId",
                table: "PurchaseOrderDetail",
                newName: "IX_PurchaseOrderDetail_SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderId",
                table: "PurchaseOrderDetail",
                newName: "IX_PurchaseOrderDetail_PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetails_MaterialId",
                table: "PurchaseOrderDetail",
                newName: "IX_PurchaseOrderDetail_MaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrder",
                table: "PurchaseOrder",
                column: "PurchaseOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrderDetail",
                table: "PurchaseOrderDetail",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaterialId",
                table: "Products",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Materials_MaterialId",
                table: "Products",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products",
                column: "ProductStatusId",
                principalTable: "ProductStatuses",
                principalColumn: "Status_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_Providers_ProviderId",
                table: "PurchaseOrder",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetail_Materials_MaterialId",
                table: "PurchaseOrderDetail",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetail_PurchaseOrder_PurchaseOrderId",
                table: "PurchaseOrderDetail",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrder",
                principalColumn: "PurchaseOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetail_Sizes_SizeId",
                table: "PurchaseOrderDetail",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderReceipt_PurchaseOrder_PurchaseOrdersPurchaseOrd~",
                table: "PurchaseOrderReceipt",
                column: "PurchaseOrdersPurchaseOrderId",
                principalTable: "PurchaseOrder",
                principalColumn: "PurchaseOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_Sizes_SizeId",
                table: "ReceiptDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
