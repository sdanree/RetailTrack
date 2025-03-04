using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class attpurchaseOrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderExternalCode",
                table: "PurchaseOrder");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderNumber",
                table: "PurchaseOrder",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "PurchaseOrder");

            migrationBuilder.AddColumn<string>(
                name: "OrderExternalCode",
                table: "PurchaseOrder",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
