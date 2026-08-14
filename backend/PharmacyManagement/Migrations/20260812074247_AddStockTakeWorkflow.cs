using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTakeWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "StockTake",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedBy",
                table: "StockTake",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdjusted",
                table: "StockTake",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StockTake",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCode",
                table: "StockAdjustmentItem",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StockTakeItemID",
                table: "StockAdjustmentItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "StockAdjustment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedBy",
                table: "StockAdjustment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCode",
                table: "DestroyReceiptItem",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StockTakeItemID",
                table: "DestroyReceiptItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "DestroyReceiptItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "DestroyReceipt",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedBy",
                table: "DestroyReceipt",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItem_StockTakeItemID",
                table: "StockAdjustmentItem",
                column: "StockTakeItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceiptItem_StockTakeItemID",
                table: "DestroyReceiptItem",
                column: "StockTakeItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_DestroyReceiptItem_StockTakeItem_StockTakeItemID",
                table: "DestroyReceiptItem",
                column: "StockTakeItemID",
                principalTable: "StockTakeItem",
                principalColumn: "StockTakeItemID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustmentItem_StockTakeItem_StockTakeItemID",
                table: "StockAdjustmentItem",
                column: "StockTakeItemID",
                principalTable: "StockTakeItem",
                principalColumn: "StockTakeItemID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
INSERT INTO [Permission] ([Name], [Description])
SELECT P.[Name], P.[Description]
FROM (VALUES
    (N'STOCKTAKE_CREATE', N'Create stock take'),
    (N'STOCKTAKE_UPDATE', N'Complete or cancel stock take'),
    (N'STOCKTAKE_VIEW', N'View stock take'),
    (N'STOCKTAKE_APPROVE', N'Approve stock take'),
    (N'STOCK_ADJUSTMENT_CREATE', N'Create stock adjustment'),
    (N'STOCK_ADJUSTMENT_VIEW', N'View stock adjustment'),
    (N'STOCK_ADJUSTMENT_APPROVE', N'Approve stock adjustment'),
    (N'DESTROY_CREATE', N'Create destroy receipt'),
    (N'DESTROY_VIEW', N'View destroy receipt'),
    (N'DESTROY_APPROVE', N'Approve destroy receipt')
) AS P ([Name], [Description])
WHERE NOT EXISTS (SELECT 1 FROM [Permission] WHERE [Name] = P.[Name]);

INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT r.[RoleID], p.[PermissionID]
FROM [Role] r
CROSS JOIN [Permission] p
WHERE r.[RoleName] IN (N'admin', N'manage_supply')
  AND p.[Name] IN (N'STOCKTAKE_CREATE', N'STOCKTAKE_UPDATE', N'STOCKTAKE_VIEW', N'STOCKTAKE_APPROVE',
                   N'STOCK_ADJUSTMENT_CREATE', N'STOCK_ADJUSTMENT_VIEW', N'STOCK_ADJUSTMENT_APPROVE',
                   N'DESTROY_CREATE', N'DESTROY_VIEW', N'DESTROY_APPROVE')
  AND NOT EXISTS (SELECT 1 FROM [RolePermission] rp WHERE rp.[RoleID] = r.[RoleID] AND rp.[PermissionID] = p.[PermissionID]);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DestroyReceiptItem_StockTakeItem_StockTakeItemID",
                table: "DestroyReceiptItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustmentItem_StockTakeItem_StockTakeItemID",
                table: "StockAdjustmentItem");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustmentItem_StockTakeItemID",
                table: "StockAdjustmentItem");

            migrationBuilder.DropIndex(
                name: "IX_DestroyReceiptItem_StockTakeItemID",
                table: "DestroyReceiptItem");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "StockTake");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "StockTake");

            migrationBuilder.DropColumn(
                name: "IsAdjusted",
                table: "StockTake");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StockTake");

            migrationBuilder.DropColumn(
                name: "ReasonCode",
                table: "StockAdjustmentItem");

            migrationBuilder.DropColumn(
                name: "StockTakeItemID",
                table: "StockAdjustmentItem");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "StockAdjustment");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "StockAdjustment");

            migrationBuilder.DropColumn(
                name: "ReasonCode",
                table: "DestroyReceiptItem");

            migrationBuilder.DropColumn(
                name: "StockTakeItemID",
                table: "DestroyReceiptItem");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "DestroyReceiptItem");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "DestroyReceipt");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "DestroyReceipt");

            migrationBuilder.Sql(@"
DELETE rp
FROM [RolePermission] rp
INNER JOIN [Permission] p ON p.[PermissionID] = rp.[PermissionID]
WHERE p.[Name] IN (N'STOCKTAKE_CREATE', N'STOCKTAKE_UPDATE', N'STOCKTAKE_VIEW', N'STOCKTAKE_APPROVE',
                   N'STOCK_ADJUSTMENT_CREATE', N'STOCK_ADJUSTMENT_VIEW', N'STOCK_ADJUSTMENT_APPROVE',
                   N'DESTROY_CREATE', N'DESTROY_VIEW', N'DESTROY_APPROVE');

DELETE FROM [Permission]
WHERE [Name] IN (N'STOCKTAKE_CREATE', N'STOCKTAKE_UPDATE', N'STOCKTAKE_VIEW', N'STOCKTAKE_APPROVE',
                 N'STOCK_ADJUSTMENT_CREATE', N'STOCK_ADJUSTMENT_VIEW', N'STOCK_ADJUSTMENT_APPROVE',
                 N'DESTROY_CREATE', N'DESTROY_VIEW', N'DESTROY_APPROVE');
");
        }
    }
}
