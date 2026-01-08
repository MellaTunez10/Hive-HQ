using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveHQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkServiceToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "Services",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "Inventory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_InventoryItemId",
                table: "Services",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Inventory_InventoryItemId",
                table: "Services",
                column: "InventoryItemId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Inventory_InventoryItemId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_InventoryItemId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Inventory");
        }
    }
}
