using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableToProductStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 4);

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "ProductStocks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 1,
                column: "Status_Name",
                value: "Habilitado");

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 2,
                column: "Status_Name",
                value: "Sin stock");

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 3,
                column: "Status_Name",
                value: "Descontinuado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "ProductStocks");

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 1,
                column: "Status_Name",
                value: "En Producción");

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 2,
                column: "Status_Name",
                value: "Listo Para Venta");

            migrationBuilder.UpdateData(
                table: "ProductStatuses",
                keyColumn: "Status_Id",
                keyValue: 3,
                column: "Status_Name",
                value: "Vendido");

            migrationBuilder.InsertData(
                table: "ProductStatuses",
                columns: new[] { "Status_Id", "Status_Name" },
                values: new object[] { 4, "Devuelto" });
        }
    }
}
