using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTakeAdjustDestroyFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAdjusted",
                table: "StockTake",
                newName: "IsAdjust");

            migrationBuilder.AddColumn<bool>(
                name: "IsDestroy",
                table: "StockTake",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdjust",
                table: "StockTakeItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDestroy",
                table: "StockTakeItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdjust",
                table: "StockTakeItem");

            migrationBuilder.DropColumn(
                name: "IsDestroy",
                table: "StockTakeItem");

            migrationBuilder.DropColumn(
                name: "IsDestroy",
                table: "StockTake");

            migrationBuilder.RenameColumn(
                name: "IsAdjust",
                table: "StockTake",
                newName: "IsAdjusted");
        }
    }
}
