using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoice_BranchID",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipt_BranchID",
                table: "GoodsReceipt");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BranchID_CreatedAt",
                table: "Invoice",
                columns: new[] { "BranchID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_BranchID_ReceiptDate",
                table: "GoodsReceipt",
                columns: new[] { "BranchID", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Batch_ExpiryDate",
                table: "Batch",
                column: "ExpiryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoice_BranchID_CreatedAt",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipt_BranchID_ReceiptDate",
                table: "GoodsReceipt");

            migrationBuilder.DropIndex(
                name: "IX_Batch_ExpiryDate",
                table: "Batch");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BranchID",
                table: "Invoice",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_BranchID",
                table: "GoodsReceipt",
                column: "BranchID");
        }
    }
}
