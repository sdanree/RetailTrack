using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToDesing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Designs",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Designs");
        }
    }
}
