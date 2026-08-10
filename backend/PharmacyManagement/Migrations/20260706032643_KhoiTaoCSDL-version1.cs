using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTaoCSDLversion1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerType",
                columns: table => new
                {
                    CustomerTypeID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerType", x => x.CustomerTypeID);
                });

            migrationBuilder.CreateTable(
                name: "ManuFacturer",
                columns: table => new
                {
                    ManufacturerID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManuFacturer", x => x.ManufacturerID);
                });

            migrationBuilder.CreateTable(
                name: "MedicineCategory",
                columns: table => new
                {
                    MedicineCategoryID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineCategory", x => x.MedicineCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "PriceList",
                columns: table => new
                {
                    PriceListID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceList", x => x.PriceListID);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    PromotionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.PromotionID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SupplierID1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.SupplierID);
                    table.ForeignKey(
                        name: "FK_Supplier_Supplier_SupplierID1",
                        column: x => x.SupplierID1,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID");
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    UnitID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.UnitID);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CustomerTypeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK_Customer_CustomerType_CustomerTypeID",
                        column: x => x.CustomerTypeID,
                        principalTable: "CustomerType",
                        principalColumn: "CustomerTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    BranchID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PriceListID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.BranchID);
                    table.ForeignKey(
                        name: "FK_Branch_PriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalTable: "PriceList",
                        principalColumn: "PriceListID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    MedicineID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DefaultRetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultWholesalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VATPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryID = table.Column<long>(type: "bigint", nullable: false),
                    ManufacturerID = table.Column<long>(type: "bigint", nullable: false),
                    BaseUnitID = table.Column<long>(type: "bigint", nullable: false),
                    MedicineCategoryID = table.Column<long>(type: "bigint", nullable: true),
                    UnitID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_Medicine_ManuFacturer_ManufacturerID",
                        column: x => x.ManufacturerID,
                        principalTable: "ManuFacturer",
                        principalColumn: "ManufacturerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medicine_MedicineCategory_MedicineCategoryID",
                        column: x => x.MedicineCategoryID,
                        principalTable: "MedicineCategory",
                        principalColumn: "MedicineCategoryID");
                    table.ForeignKey(
                        name: "FK_Medicine_Unit_UnitID",
                        column: x => x.UnitID,
                        principalTable: "Unit",
                        principalColumn: "UnitID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerDebtSummary",
                columns: table => new
                {
                    CustomerDebtSummaryID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Month = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Increase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDebtSummary", x => x.CustomerDebtSummaryID);
                    table.ForeignKey(
                        name: "FK_CustomerDebtSummary_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                columns: table => new
                {
                    ReceiptID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_Receipt_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WareHouse",
                columns: table => new
                {
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    WarehouseType = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WareHouse", x => x.WarehouseID);
                    table.ForeignKey(
                        name: "FK_WareHouse_Branch_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branch",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogID);
                    table.ForeignKey(
                        name: "FK_AuditLog_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceipt",
                columns: table => new
                {
                    GoodsReceiptID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<long>(type: "bigint", nullable: false),
                    SupplierID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipt", x => x.GoodsReceiptID);
                    table.ForeignKey(
                        name: "FK_GoodsReceipt_Branch_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branch",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceipt_Supplier_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceipt_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoice_Branch_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branch",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoice_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoice_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReturn",
                columns: table => new
                {
                    PurchaseReturnID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReturn", x => x.PurchaseReturnID);
                    table.ForeignKey(
                        name: "FK_PurchaseReturn_Supplier_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseReturn_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    RefreshTokenID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.RefreshTokenID);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturn",
                columns: table => new
                {
                    SalesReturnID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturn", x => x.SalesReturnID);
                    table.ForeignKey(
                        name: "FK_SalesReturn_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesReturn_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBranch",
                columns: table => new
                {
                    UserBranchID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranch", x => x.UserBranchID);
                    table.ForeignKey(
                        name: "FK_UserBranch_Branch_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branch",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranch_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceListItem",
                columns: table => new
                {
                    PriceListItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListID = table.Column<long>(type: "bigint", nullable: false),
                    MedicineID = table.Column<long>(type: "bigint", nullable: false),
                    RetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WholesalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListItem", x => x.PriceListItemID);
                    table.ForeignKey(
                        name: "FK_PriceListItem_Medicine_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceListItem_PriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalTable: "PriceList",
                        principalColumn: "PriceListID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionItem",
                columns: table => new
                {
                    PromotionItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionID = table.Column<long>(type: "bigint", nullable: false),
                    MedicineID = table.Column<long>(type: "bigint", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionItem", x => x.PromotionItemID);
                    table.ForeignKey(
                        name: "FK_PromotionItem_Medicine_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionItem_Promotion_PromotionID",
                        column: x => x.PromotionID,
                        principalTable: "Promotion",
                        principalColumn: "PromotionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitConversion",
                columns: table => new
                {
                    UnitConversionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitID = table.Column<long>(type: "bigint", nullable: false),
                    MedicineID = table.Column<long>(type: "bigint", nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitConversion", x => x.UnitConversionID);
                    table.ForeignKey(
                        name: "FK_UnitConversion_Medicine_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitConversion_Unit_UnitID",
                        column: x => x.UnitID,
                        principalTable: "Unit",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DestroyReceipt",
                columns: table => new
                {
                    DestroyReceiptID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestroyReceipt", x => x.DestroyReceiptID);
                    table.ForeignKey(
                        name: "FK_DestroyReceipt_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DestroyReceipt_WareHouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "WareHouse",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTake",
                columns: table => new
                {
                    StockTakeID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsBalance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTake", x => x.StockTakeID);
                    table.ForeignKey(
                        name: "FK_StockTake_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTake_WareHouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "WareHouse",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItem",
                columns: table => new
                {
                    GoodsReceiptItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptID = table.Column<long>(type: "bigint", nullable: false),
                    MedicineID = table.Column<long>(type: "bigint", nullable: false),
                    UnitID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptItem", x => x.GoodsReceiptItemID);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItem_GoodsReceipt_GoodsReceiptID",
                        column: x => x.GoodsReceiptID,
                        principalTable: "GoodsReceipt",
                        principalColumn: "GoodsReceiptID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItem_Medicine_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicine",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItem_Unit_UnitID",
                        column: x => x.UnitID,
                        principalTable: "Unit",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustment",
                columns: table => new
                {
                    StockAdjustmentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    StockTakeID = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustment", x => x.StockAdjustmentID);
                    table.ForeignKey(
                        name: "FK_StockAdjustment_StockTake_StockTakeID",
                        column: x => x.StockTakeID,
                        principalTable: "StockTake",
                        principalColumn: "StockTakeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAdjustment_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustment_WareHouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "WareHouse",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batch",
                columns: table => new
                {
                    BatchID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<long>(type: "bigint", nullable: false),
                    GoodsReceiptItemID = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityInStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batch", x => x.BatchID);
                    table.ForeignKey(
                        name: "FK_Batch_GoodsReceiptItem_GoodsReceiptItemID",
                        column: x => x.GoodsReceiptItemID,
                        principalTable: "GoodsReceiptItem",
                        principalColumn: "GoodsReceiptItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Batch_WareHouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "WareHouse",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DestroyReceiptItem",
                columns: table => new
                {
                    DestroyReceiptItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestroyReceiptID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestroyReceiptItem", x => x.DestroyReceiptItemID);
                    table.ForeignKey(
                        name: "FK_DestroyReceiptItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DestroyReceiptItem_DestroyReceipt_DestroyReceiptID",
                        column: x => x.DestroyReceiptID,
                        principalTable: "DestroyReceipt",
                        principalColumn: "DestroyReceiptID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransaction",
                columns: table => new
                {
                    InventoryTransactionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceID = table.Column<long>(type: "bigint", nullable: false),
                    ActionBy = table.Column<long>(type: "bigint", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransaction", x => x.InventoryTransactionID);
                    table.ForeignKey(
                        name: "FK_InventoryTransaction_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTransaction_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItem",
                columns: table => new
                {
                    InvoiceItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    UnitID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItem", x => x.InvoiceItemID);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Invoice_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Unit_UnitID",
                        column: x => x.UnitID,
                        principalTable: "Unit",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReturnItem",
                columns: table => new
                {
                    PurchaseReturnItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseReturnID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReturnItem", x => x.PurchaseReturnItemID);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnItem_PurchaseReturn_PurchaseReturnID",
                        column: x => x.PurchaseReturnID,
                        principalTable: "PurchaseReturn",
                        principalColumn: "PurchaseReturnID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnItem",
                columns: table => new
                {
                    SalesReturnItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesReturnID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturnItem", x => x.SalesReturnItemID);
                    table.ForeignKey(
                        name: "FK_SalesReturnItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesReturnItem_SalesReturn_SalesReturnID",
                        column: x => x.SalesReturnID,
                        principalTable: "SalesReturn",
                        principalColumn: "SalesReturnID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentItem",
                columns: table => new
                {
                    StockAdjustmentItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustmentID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    AdjustQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustmentItem", x => x.StockAdjustmentItemID);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItem_StockAdjustment_StockAdjustmentID",
                        column: x => x.StockAdjustmentID,
                        principalTable: "StockAdjustment",
                        principalColumn: "StockAdjustmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTakeItem",
                columns: table => new
                {
                    StockTakeItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTakeID = table.Column<long>(type: "bigint", nullable: false),
                    BatchID = table.Column<long>(type: "bigint", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DifferenceQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeItem", x => x.StockTakeItemID);
                    table.ForeignKey(
                        name: "FK_StockTakeItem_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTakeItem_StockTake_StockTakeID",
                        column: x => x.StockTakeID,
                        principalTable: "StockTake",
                        principalColumn: "StockTakeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserID",
                table: "AuditLog",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Batch_GoodsReceiptItemID",
                table: "Batch",
                column: "GoodsReceiptItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Batch_WarehouseID",
                table: "Batch",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_PriceListID",
                table: "Branch",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerTypeID",
                table: "Customer",
                column: "CustomerTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDebtSummary_CustomerID",
                table: "CustomerDebtSummary",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceipt_UserID",
                table: "DestroyReceipt",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceipt_WarehouseID",
                table: "DestroyReceipt",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceiptItem_BatchID",
                table: "DestroyReceiptItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_DestroyReceiptItem_DestroyReceiptID",
                table: "DestroyReceiptItem",
                column: "DestroyReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_BranchID",
                table: "GoodsReceipt",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_SupplierID",
                table: "GoodsReceipt",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_UserID",
                table: "GoodsReceipt",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItem_GoodsReceiptID",
                table: "GoodsReceiptItem",
                column: "GoodsReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItem_MedicineID",
                table: "GoodsReceiptItem",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItem_UnitID",
                table: "GoodsReceiptItem",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_BatchID",
                table: "InventoryTransaction",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_UserID",
                table: "InventoryTransaction",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BranchID",
                table: "Invoice",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerID",
                table: "Invoice",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_UserID",
                table: "Invoice",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_BatchID",
                table: "InvoiceItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_InvoiceID",
                table: "InvoiceItem",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_UnitID",
                table: "InvoiceItem",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_ManufacturerID",
                table: "Medicine",
                column: "ManufacturerID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_MedicineCategoryID",
                table: "Medicine",
                column: "MedicineCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_UnitID",
                table: "Medicine",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_MedicineID",
                table: "PriceListItem",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_PriceListID",
                table: "PriceListItem",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItem_MedicineID",
                table: "PromotionItem",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItem_PromotionID",
                table: "PromotionItem",
                column: "PromotionID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturn_SupplierID",
                table: "PurchaseReturn",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturn_UserID",
                table: "PurchaseReturn",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnItem_BatchID",
                table: "PurchaseReturnItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnItem_PurchaseReturnID",
                table: "PurchaseReturnItem",
                column: "PurchaseReturnID");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_CustomerID",
                table: "Receipt",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserID",
                table: "RefreshToken",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturn_CustomerID",
                table: "SalesReturn",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturn_UserID",
                table: "SalesReturn",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItem_BatchID",
                table: "SalesReturnItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItem_SalesReturnID",
                table: "SalesReturnItem",
                column: "SalesReturnID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustment_StockTakeID",
                table: "StockAdjustment",
                column: "StockTakeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustment_UserID",
                table: "StockAdjustment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustment_WarehouseID",
                table: "StockAdjustment",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItem_BatchID",
                table: "StockAdjustmentItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItem_StockAdjustmentID",
                table: "StockAdjustmentItem",
                column: "StockAdjustmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTake_UserID",
                table: "StockTake",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTake_WarehouseID",
                table: "StockTake",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeItem_BatchID",
                table: "StockTakeItem",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeItem_StockTakeID",
                table: "StockTakeItem",
                column: "StockTakeID");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_SupplierID1",
                table: "Supplier",
                column: "SupplierID1");

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversion_MedicineID",
                table: "UnitConversion",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversion_UnitID",
                table: "UnitConversion",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleID",
                table: "User",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranch_BranchID",
                table: "UserBranch",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranch_UserID",
                table: "UserBranch",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouse_BranchID",
                table: "WareHouse",
                column: "BranchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "CustomerDebtSummary");

            migrationBuilder.DropTable(
                name: "DestroyReceiptItem");

            migrationBuilder.DropTable(
                name: "InventoryTransaction");

            migrationBuilder.DropTable(
                name: "InvoiceItem");

            migrationBuilder.DropTable(
                name: "PriceListItem");

            migrationBuilder.DropTable(
                name: "PromotionItem");

            migrationBuilder.DropTable(
                name: "PurchaseReturnItem");

            migrationBuilder.DropTable(
                name: "Receipt");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "SalesReturnItem");

            migrationBuilder.DropTable(
                name: "StockAdjustmentItem");

            migrationBuilder.DropTable(
                name: "StockTakeItem");

            migrationBuilder.DropTable(
                name: "UnitConversion");

            migrationBuilder.DropTable(
                name: "UserBranch");

            migrationBuilder.DropTable(
                name: "DestroyReceipt");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "PurchaseReturn");

            migrationBuilder.DropTable(
                name: "SalesReturn");

            migrationBuilder.DropTable(
                name: "StockAdjustment");

            migrationBuilder.DropTable(
                name: "Batch");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "StockTake");

            migrationBuilder.DropTable(
                name: "GoodsReceiptItem");

            migrationBuilder.DropTable(
                name: "CustomerType");

            migrationBuilder.DropTable(
                name: "WareHouse");

            migrationBuilder.DropTable(
                name: "GoodsReceipt");

            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ManuFacturer");

            migrationBuilder.DropTable(
                name: "MedicineCategory");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "PriceList");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
