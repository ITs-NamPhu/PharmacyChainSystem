using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTakeID_DestroyReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StockTakeID",
                table: "DestroyReceipt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceipt_StockTakeID",
                table: "DestroyReceipt",
                column: "StockTakeID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DestroyReceipt_StockTake_StockTakeID",
                table: "DestroyReceipt",
                column: "StockTakeID",
                principalTable: "StockTake",
                principalColumn: "StockTakeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DestroyReceipt_StockTake_StockTakeID",
                table: "DestroyReceipt");

            migrationBuilder.DropIndex(
                name: "IX_DestroyReceipt_StockTakeID",
                table: "DestroyReceipt");

            migrationBuilder.DropColumn(
                name: "StockTakeID",
                table: "DestroyReceipt");
        }
    }
}
