using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class DropBatchNumber_Batch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "InvoiceItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "GoodsReceiptItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "GoodsReceiptItem");
        }
    }
}
