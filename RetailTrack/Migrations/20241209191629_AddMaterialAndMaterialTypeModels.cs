using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialAndMaterialTypeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Designs_DesignId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DesignId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Materials",
                newName: "Stock");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialTypeId",
                table: "Materials",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Designs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_MaterialTypes_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_MaterialTypes_MaterialTypeId",
                table: "Materials");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MaterialTypeId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Materials",
                newName: "Quantity");

            migrationBuilder.UpdateData(
                table: "Designs",
                keyColumn: "Name",
                keyValue: null,
                column: "Name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Designs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DesignId",
                table: "Products",
                column: "DesignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Designs_DesignId",
                table: "Products",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
