using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtAndPaymentFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bỏ ReceiptNumber, thêm UserID (người lập phiếu thu) như một cột mới
            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "Receipt");

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "Receipt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Gán các phiếu thu cũ (mock data) về 1 user hợp lệ trước khi tạo FK
            migrationBuilder.Sql(
                "UPDATE [Receipt] SET [UserID] = 1 WHERE [UserID] = 0");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Receipt",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Chuyển cột Year/Month (DateTime) sang int có giữ dữ liệu
            migrationBuilder.AddColumn<int>(
                name: "YearInt",
                table: "CustomerDebtSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthInt",
                table: "CustomerDebtSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE [CustomerDebtSummary] SET [YearInt] = YEAR([Year]), [MonthInt] = MONTH([Month])");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "CustomerDebtSummary");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "CustomerDebtSummary");

            migrationBuilder.RenameColumn(
                name: "YearInt",
                table: "CustomerDebtSummary",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MonthInt",
                table: "CustomerDebtSummary",
                newName: "Month");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_UserID",
                table: "Receipt",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_User_UserID",
                table: "Receipt",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_User_UserID",
                table: "Receipt");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_UserID",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customer");

            // Convert Year/Month (int) trở lại DateTime (giữ dữ liệu)
            migrationBuilder.AddColumn<DateTime>(
                name: "YearDateTime",
                table: "CustomerDebtSummary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MonthDateTime",
                table: "CustomerDebtSummary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(
                "UPDATE [CustomerDebtSummary] SET [YearDateTime] = DATEFROMPARTS([Year], 1, 1), [MonthDateTime] = DATEFROMPARTS(2024, [Month], 1)");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "CustomerDebtSummary");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "CustomerDebtSummary");

            migrationBuilder.RenameColumn(
                name: "YearDateTime",
                table: "CustomerDebtSummary",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MonthDateTime",
                table: "CustomerDebtSummary",
                newName: "Month");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Receipt");

            migrationBuilder.AddColumn<long>(
                name: "ReceiptNumber",
                table: "Receipt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
