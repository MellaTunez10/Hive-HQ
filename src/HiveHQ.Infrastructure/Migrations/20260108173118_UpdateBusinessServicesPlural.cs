using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveHQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusinessServicesPlural : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Services_BusinessServiceId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Inventory_InventoryItemId",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "BusinessServices");

            migrationBuilder.RenameIndex(
                name: "IX_Services_InventoryItemId",
                table: "BusinessServices",
                newName: "IX_BusinessServices_InventoryItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessServices",
                table: "BusinessServices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessServices_Inventory_InventoryItemId",
                table: "BusinessServices",
                column: "InventoryItemId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_BusinessServices_BusinessServiceId",
                table: "Orders",
                column: "BusinessServiceId",
                principalTable: "BusinessServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessServices_Inventory_InventoryItemId",
                table: "BusinessServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_BusinessServices_BusinessServiceId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessServices",
                table: "BusinessServices");

            migrationBuilder.RenameTable(
                name: "BusinessServices",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessServices_InventoryItemId",
                table: "Services",
                newName: "IX_Services_InventoryItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Services_BusinessServiceId",
                table: "Orders",
                column: "BusinessServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Inventory_InventoryItemId",
                table: "Services",
                column: "InventoryItemId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }
    }
}
