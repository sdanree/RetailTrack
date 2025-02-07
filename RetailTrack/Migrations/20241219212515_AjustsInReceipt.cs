using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AjustsInReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptAmount",
                table: "Receipts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptAmount",
                table: "Receipts",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
