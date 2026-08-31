using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptDetailWalletAndPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoice_CustomerID",
                table: "Invoice");

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "Receipt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Receipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WalletBalance",
                table: "Customer",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CustomerWalletHistory",
                columns: table => new
                {
                    CustomerWalletHistoryID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefType = table.Column<int>(type: "int", nullable: false),
                    RefId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerWalletHistory", x => x.CustomerWalletHistoryID);
                    table.ForeignKey(
                        name: "FK_CustomerWalletHistory_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptDetail",
                columns: table => new
                {
                    ReceiptDetailID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceID = table.Column<long>(type: "bigint", nullable: false),
                    AmountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetail", x => x.ReceiptDetailID);
                    table.ForeignKey(
                        name: "FK_ReceiptDetail_Invoice_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptDetail_Receipt_ReceiptID",
                        column: x => x.ReceiptID,
                        principalTable: "Receipt",
                        principalColumn: "ReceiptID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_BranchID",
                table: "Receipt",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerID_CreatedAt",
                table: "Invoice",
                columns: new[] { "CustomerID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerWalletHistory_CustomerID_CreateDate",
                table: "CustomerWalletHistory",
                columns: new[] { "CustomerID", "CreateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetail_InvoiceID",
                table: "ReceiptDetail",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetail_ReceiptID",
                table: "ReceiptDetail",
                column: "ReceiptID");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Branch_BranchID",
                table: "Receipt",
                column: "BranchID",
                principalTable: "Branch",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Branch_BranchID",
                table: "Receipt");

            migrationBuilder.DropTable(
                name: "CustomerWalletHistory");

            migrationBuilder.DropTable(
                name: "ReceiptDetail");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_BranchID",
                table: "Receipt");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_CustomerID_CreatedAt",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerID",
                table: "Invoice",
                column: "CustomerID");
        }
    }
}
