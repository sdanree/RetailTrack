using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptExternalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la columna ReceiptExternalCode sin tocar la estructura de Provider
            migrationBuilder.AddColumn<string>(
                name: "ReceiptExternalCode",
                table: "Receipts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover la columna ReceiptExternalCode si se revierte la migración
            migrationBuilder.DropColumn(
                name: "ReceiptExternalCode",
                table: "Receipts");
        }
    }
}
