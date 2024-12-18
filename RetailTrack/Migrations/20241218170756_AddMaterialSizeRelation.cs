using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialSizeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Materials_Sizes_SizeId",
            //     table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_SizeId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Materials");

            migrationBuilder.CreateTable(
                name: "MaterialSizes",
                columns: table => new
                {
                    MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SizeId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSizes", x => new { x.MaterialId, x.SizeId });
                    table.ForeignKey(
                        name: "FK_MaterialSizes_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialSizes_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Size_Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSizes_SizeId",
                table: "MaterialSizes",
                column: "SizeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialSizes");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Materials",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_SizeId",
                table: "Materials",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Sizes_SizeId",
                table: "Materials",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Size_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
