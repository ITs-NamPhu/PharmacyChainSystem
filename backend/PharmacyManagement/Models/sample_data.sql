/*
===============================================================================
PHARMACY MANAGEMENT SYSTEM - COMPREHENSIVE SAMPLE DATA SCRIPT
===============================================================================
Compatible with EF Core 9 Schema & Latest Migrations
- Fixed Batch table structure (no BatchNumber)
- Added UnitName to GoodsReceiptItem and InvoiceItem
- Added IsDeleted soft-delete support across all entities (User, Customer, Supplier, Manufacturer, Medicine, GoodsReceipt, Invoice, Receipt)
- Support new Debt & Payment entities: Receipt, ReceiptDetail, CustomerWalletHistory, CustomerDebtSummary (int Year/Month)
- Customer Email constraint: only 'itsephu' and 'namphuits' have Email; all others are NULL
- Explicit test records for 'itsephu' (with outstanding debt) and 'namphuits' (zero debt)
- Dynamic relative dates around GETDATE() for dashboard analytics
- Consistent stock tracking, foreign keys, unique constraints on StockAdjustment/DestroyReceipt
- Rich realistic Vietnamese pharmaceutical catalog (400+ medicines, 400+ customers, 50+ suppliers)
===============================================================================
*/

USE [PharmacySystem]
GO
SET NOCOUNT ON
SET XACT_ABORT ON
BEGIN TRANSACTION

-- =============================================================================
-- 0. XOA DU LIEU CU THEO DUNG THU TU KHOA NGOAI (FK)
-- =============================================================================
PRINT '0/32 - Dang xoa du lieu cu va reset identity...'

IF OBJECT_ID(N'[ChatMessage]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [ChatMessage]; DBCC CHECKIDENT ([ChatMessage], RESEED, 0);');
END

IF OBJECT_ID(N'[ChatConversation]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [ChatConversation]; DBCC CHECKIDENT ([ChatConversation], RESEED, 0);');
END

IF OBJECT_ID(N'[RefreshToken]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [RefreshToken]; DBCC CHECKIDENT ([RefreshToken], RESEED, 0);');
END

IF OBJECT_ID(N'[AuditLog]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [AuditLog]; DBCC CHECKIDENT ([AuditLog], RESEED, 0);');
END

IF OBJECT_ID(N'[InventoryTransaction]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [InventoryTransaction]; DBCC CHECKIDENT ([InventoryTransaction], RESEED, 0);');
END

IF OBJECT_ID(N'[DestroyReceiptItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [DestroyReceiptItem]; DBCC CHECKIDENT ([DestroyReceiptItem], RESEED, 0);');
END

IF OBJECT_ID(N'[DestroyReceipt]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [DestroyReceipt]; DBCC CHECKIDENT ([DestroyReceipt], RESEED, 0);');
END

IF OBJECT_ID(N'[StockAdjustmentItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [StockAdjustmentItem]; DBCC CHECKIDENT ([StockAdjustmentItem], RESEED, 0);');
END

IF OBJECT_ID(N'[StockAdjustment]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [StockAdjustment]; DBCC CHECKIDENT ([StockAdjustment], RESEED, 0);');
END

IF OBJECT_ID(N'[StockTakeItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [StockTakeItem]; DBCC CHECKIDENT ([StockTakeItem], RESEED, 0);');
END

IF OBJECT_ID(N'[StockTake]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [StockTake]; DBCC CHECKIDENT ([StockTake], RESEED, 0);');
END

IF OBJECT_ID(N'[SalesReturnItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [SalesReturnItem]; DBCC CHECKIDENT ([SalesReturnItem], RESEED, 0);');
END

IF OBJECT_ID(N'[SalesReturn]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [SalesReturn]; DBCC CHECKIDENT ([SalesReturn], RESEED, 0);');
END

IF OBJECT_ID(N'[PurchaseReturnItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [PurchaseReturnItem]; DBCC CHECKIDENT ([PurchaseReturnItem], RESEED, 0);');
END

IF OBJECT_ID(N'[PurchaseReturn]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [PurchaseReturn]; DBCC CHECKIDENT ([PurchaseReturn], RESEED, 0);');
END

IF OBJECT_ID(N'[ReceiptDetail]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [ReceiptDetail]; DBCC CHECKIDENT ([ReceiptDetail], RESEED, 0);');
END

IF OBJECT_ID(N'[Receipt]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Receipt]; DBCC CHECKIDENT ([Receipt], RESEED, 0);');
END

IF OBJECT_ID(N'[CustomerWalletHistory]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [CustomerWalletHistory]; DBCC CHECKIDENT ([CustomerWalletHistory], RESEED, 0);');
END

IF OBJECT_ID(N'[InvoiceItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [InvoiceItem]; DBCC CHECKIDENT ([InvoiceItem], RESEED, 0);');
END

IF OBJECT_ID(N'[Invoice]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Invoice]; DBCC CHECKIDENT ([Invoice], RESEED, 0);');
END

IF OBJECT_ID(N'[Batch]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Batch]; DBCC CHECKIDENT ([Batch], RESEED, 0);');
END

IF OBJECT_ID(N'[GoodsReceiptItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [GoodsReceiptItem]; DBCC CHECKIDENT ([GoodsReceiptItem], RESEED, 0);');
END

IF OBJECT_ID(N'[GoodsReceipt]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [GoodsReceipt]; DBCC CHECKIDENT ([GoodsReceipt], RESEED, 0);');
END

IF OBJECT_ID(N'[PromotionItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [PromotionItem]; DBCC CHECKIDENT ([PromotionItem], RESEED, 0);');
END

IF OBJECT_ID(N'[Promotion]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Promotion]; DBCC CHECKIDENT ([Promotion], RESEED, 0);');
END

IF OBJECT_ID(N'[PriceListItem]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [PriceListItem]; DBCC CHECKIDENT ([PriceListItem], RESEED, 0);');
END

IF OBJECT_ID(N'[UnitConversion]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [UnitConversion]; DBCC CHECKIDENT ([UnitConversion], RESEED, 0);');
END

IF OBJECT_ID(N'[Medicine]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Medicine]; DBCC CHECKIDENT ([Medicine], RESEED, 0);');
END

IF OBJECT_ID(N'[MedicineCategory]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [MedicineCategory]; DBCC CHECKIDENT ([MedicineCategory], RESEED, 0);');
END

IF OBJECT_ID(N'[ManuFacturer]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [ManuFacturer]; DBCC CHECKIDENT ([ManuFacturer], RESEED, 0);');
END

IF OBJECT_ID(N'[Unit]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Unit]; DBCC CHECKIDENT ([Unit], RESEED, 0);');
END

IF OBJECT_ID(N'[Supplier]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Supplier]; DBCC CHECKIDENT ([Supplier], RESEED, 0);');
END

IF OBJECT_ID(N'[CustomerDebtSummary]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [CustomerDebtSummary]; DBCC CHECKIDENT ([CustomerDebtSummary], RESEED, 0);');
END

IF OBJECT_ID(N'[Customer]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Customer]; DBCC CHECKIDENT ([Customer], RESEED, 0);');
END

IF OBJECT_ID(N'[CustomerType]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [CustomerType]; DBCC CHECKIDENT ([CustomerType], RESEED, 0);');
END

IF OBJECT_ID(N'[WareHouse]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [WareHouse]; DBCC CHECKIDENT ([WareHouse], RESEED, 0);');
END

IF OBJECT_ID(N'[UserBranch]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [UserBranch]; DBCC CHECKIDENT ([UserBranch], RESEED, 0);');
END

IF OBJECT_ID(N'[User]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [User]; DBCC CHECKIDENT ([User], RESEED, 0);');
END

IF OBJECT_ID(N'[Branch]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Branch]; DBCC CHECKIDENT ([Branch], RESEED, 0);');
END

IF OBJECT_ID(N'[PriceList]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [PriceList]; DBCC CHECKIDENT ([PriceList], RESEED, 0);');
END

IF OBJECT_ID(N'[RolePermission]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [RolePermission]; DBCC CHECKIDENT ([RolePermission], RESEED, 0);');
END

IF OBJECT_ID(N'[Permission]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Permission]; DBCC CHECKIDENT ([Permission], RESEED, 0);');
END

IF OBJECT_ID(N'[Role]', N'U') IS NOT NULL
BEGIN
    EXEC(N'DELETE FROM [Role]; DBCC CHECKIDENT ([Role], RESEED, 0);');
END

-- Tuong thich: Dam bao neu cot BatchNumber ton tai tren DB thi cho phep NULL de tranh loi INSERT
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Batch]') AND name = 'BatchNumber' AND is_nullable = 0)
BEGIN
    EXEC(N'ALTER TABLE [Batch] ALTER COLUMN [BatchNumber] BIGINT NULL;');
END

-- =============================================================================
-- 1. ROLES (5 vai tro he thong)
-- =============================================================================
PRINT '1/32 - Dang tao Roles...'
INSERT INTO [Role] ([RoleName]) VALUES
('admin'),
('manage_supply'),
('manage_branch'),
('user_sale'),
('user_warehouse')

-- =============================================================================
-- 2. PERMISSIONS (Phan quyen day du cho he thong)
-- =============================================================================
PRINT '2/32 - Dang tao Permissions...'
INSERT INTO [Permission] ([Name], [Description]) VALUES
('BRANCH_VIEW', N'Xem chi nhanh'),
('BRANCH_CREATE', N'Them chi nhanh'),
('BRANCH_UPDATE', N'Cap nhat chi nhanh'),
('BRANCH_DELETE', N'Xoa chi nhanh'),
('CUSTOMER_VIEW', N'Xem khach hang'),
('CUSTOMER_CREATE', N'Them khach hang'),
('CUSTOMER_UPDATE', N'Cap nhat khach hang'),
('CUSTOMER_DELETE', N'Xoa khach hang'),
('CUSTOMERTYPE_VIEW', N'Xem loai khach hang'),
('CUSTOMERTYPE_CREATE', N'Them loai khach hang'),
('CUSTOMERTYPE_UPDATE', N'Cap nhat loai khach hang'),
('CUSTOMERTYPE_DELETE', N'Xoa loai khach hang'),
('CUSTOMER_DEBT_VIEW', N'Xem cong no khach hang'),
('CUSTOMER_DEBT_CLOSE', N'Chot so cong no khach hang'),
('RECEIPT_VIEW', N'Xem phieu thu'),
('RECEIPT_CREATE', N'Tao phieu thu'),
('RECEIPT_UPDATE', N'Cap nhat phieu thu'),
('RECEIPT_DELETE', N'Xoa phieu thu'),
('DESTROY_VIEW', N'Xem phieu tieu huy'),
('DESTROY_CREATE', N'Tao phieu tieu huy'),
('DESTROY_APPROVE', N'Duyet phieu tieu huy'),
('GOODS_RECEIPT_VIEW', N'Xem phieu nhap hang'),
('GOODS_RECEIPT_CREATE', N'Tao phieu nhap hang'),
('GOODS_RECEIPT_UPDATE', N'Cap nhat phieu nhap hang'),
('GOODS_RECEIPT_DELETE', N'Xoa phieu nhap hang'),
('INVOICE_VIEW', N'Xem hoa don ban hang'),
('INVOICE_CREATE', N'Tao hoa don ban hang'),
('INVOICE_UPDATE', N'Cap nhat hoa don ban hang'),
('INVOICE_DELETE', N'Xoa hoa don ban hang'),
('MANUFACTURER_VIEW', N'Xem nha san xuat'),
('MANUFACTURER_CREATE', N'Them nha san xuat'),
('MANUFACTURER_UPDATE', N'Cap nhat nha san xuat'),
('MANUFACTURER_DELETE', N'Xoa nha san xuat'),
('MEDICINECATEGORY_VIEW', N'Xem danh muc thuoc'),
('MEDICINECATEGORY_CREATE', N'Them danh muc thuoc'),
('MEDICINECATEGORY_UPDATE', N'Cap nhat danh muc thuoc'),
('MEDICINECATEGORY_DELETE', N'Xoa danh muc thuoc'),
('MEDICINE_VIEW', N'Xem danh sach thuoc'),
('MEDICINE_CREATE', N'Them thuoc moi'),
('MEDICINE_UPDATE', N'Cap nhat thong tin thuoc'),
('MEDICINE_DELETE', N'Xoa thuoc'),
('ROLE_VIEW', N'Xem vai tro'),
('ROLE_CREATE', N'Them vai tro'),
('ROLE_UPDATE', N'Cap nhat vai tro'),
('ROLE_DELETE', N'Xoa vai tro'),
('STOCK_ADJUSTMENT_VIEW', N'Xem phieu dieu chinh kho'),
('STOCK_ADJUSTMENT_CREATE', N'Tao phieu dieu chinh kho'),
('STOCK_ADJUSTMENT_APPROVE', N'Duyet phieu dieu chinh kho'),
('STOCK_ADJUSTMENT_DELETE', N'Xoa phieu dieu chinh kho'),
('STOCKTAKE_VIEW', N'Xem phieu kiem ke'),
('STOCKTAKE_CREATE', N'Tao phieu kiem ke'),
('STOCKTAKE_UPDATE', N'Cap nhat phieu kiem ke'),
('STOCKTAKE_APPROVE', N'Duyet phieu kiem ke'),
('STOCKTAKE_DELETE', N'Xoa phieu kiem ke'),
('SUPPLIER_VIEW', N'Xem nha cung cap'),
('SUPPLIER_CREATE', N'Them nha cung cap'),
('SUPPLIER_UPDATE', N'Cap nhat nha cung cap'),
('SUPPLIER_DELETE', N'Xoa nha cung cap'),
('UNIT_VIEW', N'Xem don vi tinh'),
('UNIT_CREATE', N'Them don vi tinh'),
('UNIT_UPDATE', N'Cap nhat don vi tinh'),
('UNIT_DELETE', N'Xoa don vi tinh'),
('UnitConversion_View', N'Xem quy doi don vi'),
('UnitConversion_Create', N'Them quy doi don vi'),
('UnitConversion_Update', N'Cap nhat quy doi don vi'),
('UnitConversion_Delete', N'Xoa quy doi don vi'),
('USER_VIEW', N'Xem nguoi dung'),
('User_View', N'Xem nguoi dung (alias)'),
('USER_CREATE', N'Them nguoi dung'),
('USER_UPDATE', N'Cap nhat nguoi dung'),
('USER_DELETE', N'Xoa nguoi dung'),
('WAREHOUSE_VIEW', N'Xem kho hang'),
('WAREHOUSE_CREATE', N'Them kho hang'),
('WAREHOUSE_UPDATE', N'Cap nhat kho hang'),
('WAREHOUSE_DELETE', N'Xoa kho hang'),
('BATCH_VIEW', N'Xem lo hang'),
('BATCH_CREATE', N'Them lo hang'),
('BATCH_UPDATE', N'Cap nhat lo hang'),
('BATCH_DELETE', N'Xoa lo hang'),
('PRICELIST_VIEW', N'Xem bang gia'),
('PRICELIST_CREATE', N'Them bang gia'),
('PRICELIST_UPDATE', N'Cap nhat bang gia'),
('PRICELIST_DELETE', N'Xoa bang gia'),
('PROMOTION_VIEW', N'Xem chuong trinh khuyen mai'),
('PROMOTION_CREATE', N'Them chuong trinh khuyen mai'),
('PROMOTION_UPDATE', N'Cap nhat chuong trinh khuyen mai'),
('PROMOTION_DELETE', N'Xoa chuong trinh khuyen mai'),
('PURCHASERETURN_VIEW', N'Xem phieu tra hang nha cung cap'),
('PURCHASERETURN_CREATE', N'Tao phieu tra hang nha cung cap'),
('PURCHASERETURN_UPDATE', N'Cap nhat phieu tra hang nha cung cap'),
('PURCHASERETURN_DELETE', N'Xoa phieu tra hang nha cung cap'),
('SALESRETURN_VIEW', N'Xem phieu khach hang tra hang'),
('SALESRETURN_CREATE', N'Tao phieu khach hang tra hang'),
('SALESRETURN_UPDATE', N'Cap nhat phieu khach hang tra hang'),
('SALESRETURN_DELETE', N'Xoa phieu khach hang tra hang'),
('PERMISSION_VIEW', N'Xem danh sach quyen'),
('PERMISSION_CREATE', N'Them quyen'),
('PERMISSION_UPDATE', N'Cap nhat quyen'),
('PERMISSION_DELETE', N'Xoa quyen')

-- =============================================================================
-- 3. ROLE PERMISSIONS (Gan quyen cho tung Role)
-- =============================================================================
PRINT '3/32 - Dang tao RolePermissions...'
-- Admin (Role 1): Toan quyen
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 1, [PermissionID] FROM [Permission]

-- Manage Supply (Role 2): Quan ly nhap hang, NCC, thuoc, kho, bang gia, khuyen mai, kiem ke, dieu chinh, huy
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 2, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'MEDICINE%' OR [Name] LIKE 'MANUFACTURER%' OR [Name] LIKE 'UNIT%'
   OR [Name] LIKE 'SUPPLIER%' OR [Name] LIKE 'GOODS_RECEIPT%' OR [Name] LIKE 'PURCHASERETURN%'
   OR [Name] LIKE 'BATCH%' OR [Name] LIKE 'WAREHOUSE%' OR [Name] LIKE 'PRICELIST%'
   OR [Name] LIKE 'PROMOTION%' OR [Name] LIKE 'STOCKTAKE%' OR [Name] LIKE 'STOCK_ADJUSTMENT%'
   OR [Name] LIKE 'DESTROY%' OR [Name] LIKE '%USER_VIEW%' OR [Name] LIKE 'PERMISSION_VIEW'

-- Manage Branch (Role 3): Quan ly ban hang, khach hang, phieu thu, cong no, kho, kiem ke tai chi nhanh
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 3, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'INVOICE%' OR [Name] LIKE 'CUSTOMER%' OR [Name] LIKE 'SALESRETURN%'
   OR [Name] LIKE 'RECEIPT%' OR [Name] LIKE 'CUSTOMER_DEBT%'
   OR [Name] LIKE 'BATCH%' OR [Name] LIKE 'WAREHOUSE%' OR [Name] LIKE 'STOCKTAKE%'
   OR [Name] LIKE 'STOCK_ADJUSTMENT%' OR [Name] LIKE 'DESTROY%' OR [Name] LIKE 'MEDICINE_VIEW%'
   OR [Name] LIKE 'UNIT_VIEW%' OR [Name] LIKE '%USER_VIEW%' OR [Name] LIKE 'PROMOTION_VIEW'

-- User Sale (Role 4): Nhan vien ban hang (lap hoa don, phieu thu, tra hang, tra cuu)
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 4, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'INVOICE%' OR [Name] LIKE 'CUSTOMER%' OR [Name] LIKE 'SALESRETURN%'
   OR [Name] LIKE 'RECEIPT%' OR [Name] LIKE 'CUSTOMER_DEBT_VIEW%'
   OR [Name] LIKE 'MEDICINE_VIEW%' OR [Name] LIKE 'UNIT_VIEW%' OR [Name] LIKE 'BATCH_VIEW%'
   OR [Name] LIKE 'PROMOTION_VIEW'

-- User Warehouse (Role 5): Nhan vien thu kho
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 5, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'GOODS_RECEIPT%' OR [Name] LIKE 'PURCHASERETURN%' OR [Name] LIKE 'BATCH%'
   OR [Name] LIKE 'WAREHOUSE%' OR [Name] LIKE 'STOCKTAKE%' OR [Name] LIKE 'STOCK_ADJUSTMENT%'
   OR [Name] LIKE 'DESTROY%' OR [Name] LIKE 'MEDICINE_VIEW%' OR [Name] LIKE 'UNIT_VIEW%'
   OR [Name] LIKE 'SUPPLIER_VIEW%'

-- =============================================================================
-- 4. PRICELIST (Bang gia chung toan he thong)
-- =============================================================================
PRINT '4/32 - Dang tao PriceList...'
INSERT INTO [PriceList] ([PriceListName], [StartDate], [EndDate]) VALUES
(N'Bảng giá chuẩn toàn hệ thống', '2024-01-01', '2030-12-31')

-- =============================================================================
-- 5. BRANCHES (4 chi nhanh)
-- =============================================================================
PRINT '5/32 - Dang tao Branches...'
INSERT INTO [Branch] ([BranchName], [Phone], [Address], [CreatedAt], [UpdatedAt], [IsActive], [PriceListID]) VALUES
(N'Chi nhánh Hà Nội', '0243123456', N'123 Lê Lợi, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội', DATEADD(year, -2, GETDATE()), GETDATE(), 1, 1),
(N'Chi nhánh TP.HCM', '0283123456', N'456 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh', DATEADD(year, -2, GETDATE()), GETDATE(), 1, 1),
(N'Chi nhánh Đà Nẵng', '0236123456', N'789 Trần Hưng Đạo, Phường An Hải Bắc, Quận Sơn Trà, Đà Nẵng', DATEADD(year, -1, GETDATE()), GETDATE(), 1, 1),
(N'Chi nhánh Cần Thơ', '0292123456', N'321 Đường 30 Tháng 4, Phường Xuân Khánh, Quận Ninh Kiều, Cần Thơ', DATEADD(year, -1, GETDATE()), GETDATE(), 1, 1)

-- =============================================================================
-- 6. USERS (14 tai khoan dung chuan BCrypt password: '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING')
-- =============================================================================
PRINT '6/32 - Dang tao Users...'
INSERT INTO [User] ([UserName], [PasswordHash], [FullName], [Phone], [Address], [Email], [IsActive], [FailedLoginCount], [IsDeleted]) VALUES
('admin', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Nguyễn Văn Admin', '0900000001', N'123 Lê Lợi, Hoàn Kiếm, Hà Nội', 'admin@pharmacymanagement.vn', 1, 0, 0),
('manage_supply', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Trần Thị Cung Ứng', '0900000002', N'456 Nguyễn Huệ, Quận 1, TP.HCM', 'supply@pharmacymanagement.vn', 1, 0, 0),
('manager_hn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Lê Hoàng Minh (QL Hà Nội)', '0900000003', N'12 Đội Cấn, Ba Đình, Hà Nội', 'manager.hn@pharmacymanagement.vn', 1, 0, 0),
('manager_hcm', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Phạm Đức Long (QL HCM)', '0900000004', N'88 Hai Bà Trưng, Quận 1, TP.HCM', 'manager.hcm@pharmacymanagement.vn', 1, 0, 0),
('manager_dn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Hoàng Thị Mai (QL Đà Nẵng)', '0900000005', N'45 Nguyễn Văn Linh, Hải Châu, Đà Nẵng', 'manager.dn@pharmacymanagement.vn', 1, 0, 0),
('manager_ct', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Phan Văn Hùng (QL Cần Thơ)', '0900000006', N'79 Hòa Bình, Ninh Kiều, Cần Thơ', 'manager.ct@pharmacymanagement.vn', 1, 0, 0),
('sale_hn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Vũ Thị Bích (Bán hàng HN)', '0900000007', N'55 Hàng Bông, Hoàn Kiếm, Hà Nội', 'sale.hn@pharmacymanagement.vn', 1, 0, 0),
('sale_hcm', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Võ Minh Trí (Bán hàng HCM)', '0900000008', N'102 Lê Lai, Quận 1, TP.HCM', 'sale.hcm@pharmacymanagement.vn', 1, 0, 0),
('sale_dn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Đặng Thị Thanh (Bán hàng ĐN)', '0900000009', N'12 Bạch Đằng, Hải Châu, Đà Nẵng', 'sale.dn@pharmacymanagement.vn', 1, 0, 0),
('sale_ct', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Bùi Văn Tài (Bán hàng CT)', '0900000010', N'34 Mậu Thân, Ninh Kiều, Cần Thơ', 'sale.ct@pharmacymanagement.vn', 1, 0, 0),
('warehouse_hn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Đỗ Thị Thu (Thủ kho HN)', '0900000011', N'90 Giải Phóng, Đống Đa, Hà Nội', 'warehouse.hn@pharmacymanagement.vn', 1, 0, 0),
('warehouse_hcm', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Hồ Thị Phương (Thủ kho HCM)', '0900000012', N'230 Cộng Hòa, Tân Bình, TP.HCM', 'warehouse.hcm@pharmacymanagement.vn', 1, 0, 0),
('warehouse_dn', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Nguyễn Minh Tuấn (Thủ kho ĐN)', '0900000013', N'67 Điện Biên Phủ, Thanh Khê, Đà Nẵng', 'warehouse.dn@pharmacymanagement.vn', 1, 0, 0),
('warehouse_ct', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Trần Văn Nam (Thủ kho CT)', '0900000014', N'15 Cách Mạng Tháng 8, Ninh Kiều, Cần Thơ', 'warehouse.ct@pharmacymanagement.vn', 1, 0, 0)

-- =============================================================================
-- 7. USERBRANCH (Phan bo nguoi dung vao cac chi nhanh)
-- =============================================================================
PRINT '7/32 - Dang tao UserBranch...'
-- Admin (UserID 1): Co quyen tai ca 4 chi nhanh, chi nhanh 1 la mac dinh
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault]) VALUES
(1, 1, 1, 1), (1, 2, 1, 0), (1, 3, 1, 0), (1, 4, 1, 0),
-- Manage Supply (UserID 2): Co quyen quan ly cung ung ca 4 chi nhanh, chi nhanh 1 la mac dinh
(2, 1, 2, 1), (2, 2, 2, 0), (2, 3, 2, 0), (2, 4, 2, 0),
-- Quan ly chi nhanh (UserID 3->CN1, 4->CN2, 5->CN3, 6->CN4)
(3, 1, 3, 1), (4, 2, 3, 1), (5, 3, 3, 1), (6, 4, 3, 1),
-- Nhan vien ban hang (UserID 7->CN1, 8->CN2, 9->CN3, 10->CN4)
(7, 1, 4, 1), (8, 2, 4, 1), (9, 3, 4, 1), (10, 4, 4, 1),
-- Nhan vien thu kho (UserID 11->CN1, 12->CN2, 13->CN3, 14->CN4)
(11, 1, 5, 1), (12, 2, 5, 1), (13, 3, 5, 1), (14, 4, 5, 1)

-- =============================================================================
-- 8. WAREHOUSES (1 kho chinh cho moi chi nhanh)
-- =============================================================================
PRINT '8/32 - Dang tao Warehouses...'
INSERT INTO [WareHouse] ([BranchID], [WarehouseName], [WarehouseType]) VALUES
(1, N'Kho Tổng Dược Phẩm - Hà Nội', 1),
(2, N'Kho Tổng Dược Phẩm - TP.HCM', 1),
(3, N'Kho Tổng Dược Phẩm - Đà Nẵng', 1),
(4, N'Kho Tổng Dược Phẩm - Cần Thơ', 1)

-- =============================================================================
-- 9. CUSTOMER TYPES (4 loai khach hang)
-- =============================================================================
PRINT '9/32 - Dang tao CustomerType...'
INSERT INTO [CustomerType] ([TypeName], [DiscountPercent]) VALUES
(N'VIP', 5.00),
(N'Khách hàng thân thiết', 2.00),
(N'Khách hàng thường', 0.00),
(N'Đại lý / Nhà thuốc liên kết', 8.00)

-- =============================================================================
-- 10. CUSTOMERS (400 khach hang; chi 2 dong co email: itsephu, namphuits)
-- =============================================================================
PRINT '10/32 - Dang tao Customers...'
DECLARE @c INT = 1
DECLARE @c_name NVARCHAR(255)
DECLARE @c_phone NVARCHAR(10)
DECLARE @c_addr NVARCHAR(255)
DECLARE @c_email NVARCHAR(255)
DECLARE @c_type BIGINT

WHILE @c <= 400
BEGIN
    IF @c = 1
    BEGIN
        SET @c_name = N'Nguyễn Văn An (itsephu)'
        SET @c_phone = '0901234561'
        SET @c_addr = N'123 Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM'
        SET @c_email = 'itsephu'
        SET @c_type = 1 -- VIP
    END
    ELSE IF @c = 2
    BEGIN
        SET @c_name = N'Trần Thị Bích (namphuits)'
        SET @c_phone = '0901234562'
        SET @c_addr = N'456 Nguyễn Huệ, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội'
        SET @c_email = 'namphuits'
        SET @c_type = 2 -- Than thiet
    END
    ELSE
    BEGIN
        SET @c_name = CASE (@c % 20)
            WHEN 0 THEN N'Nguyễn Văn An' WHEN 1 THEN N'Trần Thị Bích' WHEN 2 THEN N'Lê Hoàng Cường'
            WHEN 3 THEN N'Phạm Thị Diệu' WHEN 4 THEN N'Hoàng Đức Em' WHEN 5 THEN N'Võ Thị Phương'
            WHEN 6 THEN N'Phan Minh Giang' WHEN 7 THEN N'Đặng Văn Hải' WHEN 8 THEN N'Bùi Thị Kiều'
            WHEN 9 THEN N'Đỗ Quốc Long' WHEN 10 THEN N'Nguyễn Thị Mai' WHEN 11 THEN N'Trần Văn Nam'
            WHEN 12 THEN N'Lê Thanh Oanh' WHEN 13 THEN N'Phạm Thị Quyên' WHEN 14 THEN N'Võ Văn Sơn'
            WHEN 15 THEN N'Trịnh Thị Thảo' WHEN 16 THEN N'Dương Văn Uy' WHEN 17 THEN N'Lý Thị Vân'
            WHEN 18 THEN N'Ngô Văn Xuân' ELSE N'Đinh Thị Yến'
        END + ' - KH' + RIGHT('000' + CAST(@c AS NVARCHAR), 4)
        SET @c_phone = '09' + RIGHT('00000000' + CAST((@c * 1234567 + 890123) % 100000000 AS NVARCHAR), 8)
        SET @c_addr = CAST((@c % 150) + 1 AS NVARCHAR) + N' Đường ' + 
            CASE (@c % 6)
                WHEN 0 THEN N'Lê Lợi, Quận 1, TP.HCM'
                WHEN 1 THEN N'Giải Phóng, Đống Đa, Hà Nội'
                WHEN 2 THEN N'Nguyễn Văn Linh, Hải Châu, Đà Nẵng'
                WHEN 3 THEN N'30 Tháng 4, Ninh Kiều, Cần Thơ'
                WHEN 4 THEN N'Cách Mạng Tháng 8, Quận 3, TP.HCM'
                ELSE N'Cầu Giấy, Cầu Giấy, Hà Nội'
            END
        SET @c_email = NULL -- Field email bo trong cho tat ca khach hang con lai
        SET @c_type = CASE WHEN @c % 20 = 0 THEN 4 WHEN @c % 7 = 0 THEN 1 WHEN @c % 3 = 0 THEN 2 ELSE 3 END
    END

    INSERT INTO [Customer] ([CustomerName], [Phone], [Address], [Email], [CustomerTypeID], [WalletBalance], [IsDeleted])
    VALUES (@c_name, @c_phone, @c_addr, @c_email, @c_type, 0.00, 0)
    SET @c = @c + 1
END

-- =============================================================================
-- 11. CUSTOMER DEBT SUMMARY (Cong no: Customer 1 itsephu CON NO, Customer 2 namphuits HET NO)
-- =============================================================================
PRINT '11/32 - Dang tao CustomerDebtSummary...'
DECLARE @cur_year INT = YEAR(GETDATE())
DECLARE @cur_month INT = MONTH(GETDATE())
DECLARE @prev_year INT = YEAR(DATEADD(month, -1, GETDATE()))
DECLARE @prev_month INT = MONTH(DATEADD(month, -1, GETDATE()))

-- Customer 1 (itsephu): CON NO 4.500.000 (Phat sinh 5.500.000, da tra 1.000.000)
INSERT INTO [CustomerDebtSummary] ([CustomerID], [Year], [Month], [OpeningBalance], [Increase], [Paid], [ClosingBalance], [IsLocked])
VALUES 
(1, @prev_year, @prev_month, 0.00, 5500000.00, 1000000.00, 4500000.00, 1),
(1, @cur_year, @cur_month, 4500000.00, 0.00, 0.00, 4500000.00, 0);

-- Customer 2 (namphuits): KHONG CON NO (Phat sinh 3.500.000, da tra 3.500.000 => Du no 0)
INSERT INTO [CustomerDebtSummary] ([CustomerID], [Year], [Month], [OpeningBalance], [Increase], [Paid], [ClosingBalance], [IsLocked])
VALUES 
(2, @prev_year, @prev_month, 0.00, 3500000.00, 3500000.00, 0.00, 1),
(2, @cur_year, @cur_month, 0.00, 0.00, 0.00, 0.00, 0);

-- Cac khach hang con lai (3..150)
DECLARE @cd INT = 3
DECLARE @open_bal DECIMAL(18,2)
DECLARE @increase DECIMAL(18,2)
DECLARE @paid DECIMAL(18,2)
WHILE @cd <= 150
BEGIN
    SET @open_bal = CAST((@cd * 175000) % 5000000 AS DECIMAL(18,2))
    SET @increase = CAST((@cd * 250000) % 8000000 + 500000 AS DECIMAL(18,2))
    SET @paid = CAST(@open_bal + (@increase * 0.8) AS DECIMAL(18,2))
    INSERT INTO [CustomerDebtSummary] ([CustomerID], [Year], [Month], [OpeningBalance], [Increase], [Paid], [ClosingBalance], [IsLocked])
    VALUES (@cd, @prev_year, @prev_month, @open_bal, @increase, @paid, @open_bal + @increase - @paid, 1)
    SET @cd = @cd + 1
END

-- =============================================================================
-- 12. SUPPLIERS (50 Nha cung cap duoc pham uy tin)
-- =============================================================================
PRINT '12/32 - Dang tao Suppliers...'
INSERT INTO [Supplier] ([SupplierName], [Phone], [Email], [Address], [IsDeleted]) VALUES
(N'Công ty Cổ phần Dược Hậu Giang (DHG)', '0292389143', 'dhgpharma@dhgpharma.com.vn', N'288 Bis Nguyễn Văn Cừ, An Hòa, Ninh Kiều, Cần Thơ', 0),
(N'Công ty Cổ phần Traphaco', '0243734179', 'info@traphaco.com.vn', N'75 Yên Ninh, Ba Đình, Hà Nội', 0),
(N'Công ty Cổ phần Xuất nhập khẩu Y tế Domesco', '0277385227', 'domesco@domesco.com', N'66 Quốc lộ 30, Phường Mỹ Phú, TP. Cao Lãnh, Đồng Tháp', 0),
(N'Công ty Cổ phần Dược phẩm Imexpharm', '0277385194', 'imexpharm@imexpharm.com', N'Số 4, Đường 30/4, Phường 1, TP. Cao Lãnh, Đồng Tháp', 0),
(N'Công ty TNHH Sanofi-Aventis Việt Nam', '0283829852', 'contact.vn@sanofi.com', N'Số 10 Hàm Nghi, Bến Nghé, Quận 1, TP.HCM', 0),
(N'Công ty Cổ phần Pymepharco', '0257382322', 'pymepharco@pymepharco.com', N'166-170 Nguyễn Huệ, Tuy Hòa, Phú Yên', 0),
(N'Công ty Cổ phần Dược phẩm OPC', '0283855948', 'info@opcpharma.com', N'1017 Hồng Bàng, Phường 12, Quận 6, TP.HCM', 0),
(N'Công ty TNHH Stellapharm', '0274376747', 'contact@stellapharm.com', N'KCN VSIP 1, Thuận An, Bình Dương', 0),
(N'Công ty Cổ phần Hóa - Dược phẩm Mekophar', '0283865025', 'info@mekophar.com', N'297/5 Lý Thường Kiệt, Phường 15, Quận 11, TP.HCM', 0),
(N'Công ty Cổ phần Dược phẩm Boston Việt Nam', '0274376960', 'contact@bostonpharma.com.vn', N'Số 43, Đường số 8, KCN VSIP 1, Bình Dương', 0),
(N'Công ty Cổ phần Nam Dược', '0243767817', 'namduoc@namduoc.vn', N'Lô A7/D21 KĐT mới Cầu Giấy, Dịch Vọng Hậu, Cầu Giấy, Hà Nội', 0),
(N'Công ty TNHH Dược phẩm Shinpoong Daewoo', '0251383601', 'shinpoong@shinpoong.com.vn', N'KCN Biên Hòa 2, Đồng Nai', 0),
(N'Công ty Cổ phần Dược phẩm Tipharco', '0273387224', 'tipharco@tipharco.com.vn', N'Số 15 Đốc Binh Kiều, Phường 2, TP. Mỹ Tho, Tiền Giang', 0),
(N'Công ty Cổ phần Dược phẩm Hà Tây (Hataphar)', '0243352220', 'duochatay@hataphar.com.vn', N'10A Phố Quang Trung, Hà Đông, Hà Nội', 0),
(N'Công ty Cổ phần Dược Trung ương 3 (Foripharm)', '0236382107', 'foripharm@dtr3.com.vn', N'115 Ngô Gia Tự, Hải Châu, Đà Nẵng', 0),
(N'Công ty Cổ phần Dược phẩm Codupha', '0283855126', 'codupha@codupha.com.vn', N'334 Tô Hiến Thành, Phường 14, Quận 10, TP.HCM', 0),
(N'Công ty TNHH Zuellig Pharma Việt Nam', '0283910260', 'info@zuelligpharma.com.vn', N'Tòa nhà Mplaza, 39 Lê Duẩn, Quận 1, TP.HCM', 0),
(N'Công ty TNHH Mega Lifesciences Việt Nam', '0283812581', 'megavietnam@megawecare.com', N'Tòa nhà e-Town, 364 Cộng Hòa, Tân Bình, TP.HCM', 0),
(N'Công ty TNHH DKSH Việt Nam', '0283812580', 'healtcare.vn@dksh.com', N'Số 23 Đại lộ Độc Lập, KCN VSIP 1, Bình Dương', 0),
(N'Tổng Công ty Dược Việt Nam (Vinapharm)', '0243844346', 'vinapharm@vinapharm.com.vn', N'12 Ngô Tất Tố, Văn Miếu, Đống Đa, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #21', '0243000021', 'supplier21@pharma-distributor.vn', N'Số 63 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #22', '0243000022', 'supplier22@pharma-distributor.vn', N'Số 66 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #23', '0243000023', 'supplier23@pharma-distributor.vn', N'Số 69 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #24', '0243000024', 'supplier24@pharma-distributor.vn', N'Số 72 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #25', '0243000025', 'supplier25@pharma-distributor.vn', N'Số 75 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #26', '0243000026', 'supplier26@pharma-distributor.vn', N'Số 78 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #27', '0243000027', 'supplier27@pharma-distributor.vn', N'Số 81 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #28', '0243000028', 'supplier28@pharma-distributor.vn', N'Số 84 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #29', '0243000029', 'supplier29@pharma-distributor.vn', N'Số 87 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #30', '0243000030', 'supplier30@pharma-distributor.vn', N'Số 90 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #31', '0243000031', 'supplier31@pharma-distributor.vn', N'Số 93 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #32', '0243000032', 'supplier32@pharma-distributor.vn', N'Số 96 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #33', '0243000033', 'supplier33@pharma-distributor.vn', N'Số 99 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #34', '0243000034', 'supplier34@pharma-distributor.vn', N'Số 102 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #35', '0243000035', 'supplier35@pharma-distributor.vn', N'Số 105 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #36', '0243000036', 'supplier36@pharma-distributor.vn', N'Số 108 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #37', '0243000037', 'supplier37@pharma-distributor.vn', N'Số 111 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #38', '0243000038', 'supplier38@pharma-distributor.vn', N'Số 114 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #39', '0243000039', 'supplier39@pharma-distributor.vn', N'Số 117 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #40', '0243000040', 'supplier40@pharma-distributor.vn', N'Số 120 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #41', '0243000041', 'supplier41@pharma-distributor.vn', N'Số 123 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #42', '0243000042', 'supplier42@pharma-distributor.vn', N'Số 126 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #43', '0243000043', 'supplier43@pharma-distributor.vn', N'Số 129 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #44', '0243000044', 'supplier44@pharma-distributor.vn', N'Số 132 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #45', '0243000045', 'supplier45@pharma-distributor.vn', N'Số 135 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #46', '0243000046', 'supplier46@pharma-distributor.vn', N'Số 138 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #47', '0243000047', 'supplier47@pharma-distributor.vn', N'Số 141 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #48', '0243000048', 'supplier48@pharma-distributor.vn', N'Số 144 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #49', '0243000049', 'supplier49@pharma-distributor.vn', N'Số 147 KCN Quang Minh, Mê Linh, Hà Nội', 0),
(N'Nhà phân phối Dược phẩm Phía Bắc #50', '0243000050', 'supplier50@pharma-distributor.vn', N'Số 150 KCN Quang Minh, Mê Linh, Hà Nội', 0)

-- =============================================================================
-- 13. MEDICINE CATEGORIES (15 danh muc thuoc)
-- =============================================================================
PRINT '13/32 - Dang tao MedicineCategory...'
INSERT INTO [MedicineCategory] ([CategoryName]) VALUES
(N'Thuốc giảm đau - hạ sốt - kháng viêm (NSAIDs)'),
(N'Thuốc kháng sinh - kháng nấm - kháng virus'),
(N'Thuốc tim mạch, huyết áp & mỡ máu'),
(N'Thuốc tiêu hóa, dạ dày & đại tràng'),
(N'Thuốc hô hấp, ho & hen suyễn'),
(N'Thuốc dị ứng & kháng histamin'),
(N'Vitamin, khoáng chất & Thực phẩm bảo vệ sức khỏe'),
(N'Thuốc da liễu & bôi ngoài da'),
(N'Thuốc mắt, tai, mũi & họng'),
(N'Thuốc thần kinh, an thần & tuần hoàn não'),
(N'Thuốc tiểu đường & nội tiết'),
(N'Thuốc cơ xương khớp & gout'),
(N'Thuốc đông y & thảo dược trị liệu'),
(N'Dược mỹ phẩm & chăm sóc da chuyên sâu'),
(N'Thiết bị & vật tư y tế gia đình')

-- =============================================================================
-- 14. MANUFACTURERS (20 hang san xuat duoc pham)
-- =============================================================================
PRINT '14/32 - Dang tao Manufacturer...'
INSERT INTO [ManuFacturer] ([ManufacturerName], [IsDeleted]) VALUES
(N'DHG Pharma (Dược Hậu Giang)', 0),
(N'Traphaco', 0),
(N'Domesco Đồng Tháp', 0),
(N'Imexpharm', 0),
(N'Sanofi-Aventis Việt Nam', 0),
(N'Pymepharco (Stada)', 0),
(N'OPC Pharma', 0),
(N'Stellapharm', 0),
(N'Mekophar', 0),
(N'Boston Pharma', 0),
(N'Nam Dược', 0),
(N'Shinpoong Pharma', 0),
(N'Pfizer Việt Nam', 0),
(N'AstraZeneca', 0),
(N'GlaxoSmithKline (GSK)', 0),
(N'Novartis', 0),
(N'Abbott Laboratories', 0),
(N'Bayer', 0),
(N'Hataphar (Dược Hà Tây)', 0),
(N'Mediplantex', 0)

-- =============================================================================
-- 15. UNITS (15 don vi tinh)
-- =============================================================================
PRINT '15/32 - Dang tao Unit...'
INSERT INTO [Unit] ([UnitName]) VALUES
(N'Viên'),
(N'Vỉ'),
(N'Hộp'),
(N'Chai'),
(N'Lọ'),
(N'Gói'),
(N'Ống'),
(N'Tuýp'),
(N'Bịch'),
(N'Thùng'),
(N'Túi'),
(N'Chiếc'),
(N'Gram'),
(N'mg'),
(N'ml')

-- =============================================================================
-- 16. MEDICINES (400 thuoc thuc te voi gia va quy cach dong goi chuan)
-- =============================================================================
PRINT '16/32 - Dang tao Medicine...'
DECLARE @m INT = 1
DECLARE @m_name NVARCHAR(255)
DECLARE @m_cat BIGINT
DECLARE @m_manu BIGINT
DECLARE @m_base_u BIGINT
DECLARE @m_sale_u BIGINT
DECLARE @m_price DECIMAL(18,2)
DECLARE @m_vat DECIMAL(18,2)
WHILE @m <= 400
BEGIN
    SET @m_cat = ((@m - 1) % 15) + 1
    SET @m_manu = ((@m - 1) % 20) + 1
    SET @m_base_u = 1 -- Mac dinh: Vien
    SET @m_sale_u = 3 -- Mac dinh: Hop
    SET @m_price = CAST(((@m * 17941) % 450000) + 15000 AS DECIMAL(18,2))
    SET @m_vat = CASE (@m % 3) WHEN 0 THEN 0.00 WHEN 1 THEN 5.00 ELSE 10.00 END

    SET @m_name = CASE @m_cat
        WHEN 1 THEN -- Giam dau ha sot
            CASE (@m % 6)
                WHEN 0 THEN N'Panadol Extra Đỏ 500mg/65mg'
                WHEN 1 THEN N'Efferalgan Sủi 500mg'
                WHEN 2 THEN N'Hapacol 650mg'
                WHEN 3 THEN N'Ibuprofen Domesco 400mg'
                WHEN 4 THEN N'Aspirin pH8 500mg'
                ELSE N'Paracetamol STELLA 500mg'
            END
        WHEN 2 THEN -- Khang sinh
            CASE (@m % 6)
                WHEN 0 THEN N'Augmentin 1g (Amoxicillin/Clavulanate)'
                WHEN 1 THEN N'Klacid MR 500mg (Clarithromycin)'
                WHEN 2 THEN N'Zithromax 500mg (Azithromycin)'
                WHEN 3 THEN N'Cefixim 200mg DHG'
                WHEN 4 THEN N'Avelox 400mg (Moxifloxacin)'
                ELSE N'Ciprobay 500mg (Ciprofloxacin)'
            END
        WHEN 3 THEN -- Tim mach huyet ap
            CASE (@m % 6)
                WHEN 0 THEN N'Amlor 5mg (Amlodipine Pfizer)'
                WHEN 1 THEN N'Lipitor 20mg (Atorvastatin)'
                WHEN 2 THEN N'Coversyl 5mg (Perindopril)'
                WHEN 3 THEN N'Concor 5mg (Bisoprolol)'
                WHEN 4 THEN N'Crestor 10mg (Rosuvastatin)'
                ELSE N'Micardis 40mg (Telmisartan)'
            END
        WHEN 4 THEN -- Tieu hoa da day
            CASE (@m % 6)
                WHEN 0 THEN N'Nexium Mups 40mg (Esomeprazole)'
                WHEN 1 THEN N'Gaviscon Dual Action Hỗn Dịch'
                WHEN 2 THEN N'Phosphalugel Gói 20g (Thuốc Dạ Dày Chữ P)'
                WHEN 3 THEN N'Duspatalin 200mg (Mebeverine)'
                WHEN 4 THEN N'Enterogermina 2 Tỷ Bào Tử Lợi Khuẩn'
                ELSE N'Smecta 3g (Diosmectite)'
            END
        WHEN 5 THEN -- Ho hap
            CASE (@m % 6)
                WHEN 0 THEN N'Ventolin Inhaler 100mcg Bình Xịt'
                WHEN 1 THEN N'Singulair 10mg (Montelukast)'
                WHEN 2 THEN N'Prospan Siro Ho Trị Đờm'
                WHEN 3 THEN N'Bisolvon 8mg (Bromhexine)'
                WHEN 4 THEN N'Acemuc 200mg (Acetylcysteine)'
                ELSE N'Seretide Evohaler 25/125mcg'
            END
        WHEN 6 THEN -- Di ung
            CASE (@m % 5)
                WHEN 0 THEN N'Telfast HD 180mg (Fexofenadine)'
                WHEN 1 THEN N'Clarityne 10mg (Loratadine)'
                WHEN 2 THEN N'Zyrtec 10mg (Cetirizine)'
                WHEN 3 THEN N'Aerius 5mg (Desloratadine)'
                ELSE N'Xyzal 5mg (Levocetirizine)'
            END
        WHEN 7 THEN -- Vitamin khoang chat
            CASE (@m % 5)
                WHEN 0 THEN N'Berocca Performance Viên Sủi Bổ Sung'
                WHEN 1 THEN N'Calci D3 Boston'
                WHEN 2 THEN N'Vitamin C 500mg Dược Hậu Giang'
                WHEN 3 THEN N'Enat 400 Vitamin E Thiên Nhiên'
                ELSE N'Pharmaton Energy Bổ Thể Lực'
            END
        WHEN 8 THEN -- Da lieu
            CASE (@m % 5)
                WHEN 0 THEN N'Kem Bôi Fucidin 20g (Acid Fusidic)'
                WHEN 1 THEN N'Bactroban 2% Tuýp 15g (Mupirocin)'
                WHEN 2 THEN N'Canesten 1% Tuýp Bôi Nấm'
                WHEN 3 THEN N'Dermovate Cream 15g (Clobetasol)'
                ELSE N'Silkron Cream 10g Bôi Viêm Da'
            END
        WHEN 9 THEN -- Mat tai mui hong
            CASE (@m % 5)
                WHEN 0 THEN N'Nước Nhỏ Mắt Tobradex 5ml'
                WHEN 1 THEN N'Xịt Mũi Otrivin 0.1% 10ml'
                WHEN 2 THEN N'Thuốc Nhỏ Mắt Systane Ultra 10ml'
                WHEN 3 THEN N'Xịt Họng Betadine Antiseptic 50ml'
                ELSE N'Dung Dịch Rửa Mắt Rohto Dryaid 10ml'
            END
        WHEN 10 THEN -- Than kinh
            CASE (@m % 5)
                WHEN 0 THEN N'Nootropil 800mg (Piracetam)'
                WHEN 1 THEN N'Tanakan 40mg (Ginkgo Biloba)'
                WHEN 2 THEN N'Seduxen 5mg (Diazepam)'
                WHEN 3 THEN N'Stugeron 25mg (Cinnarizine)'
                ELSE N'Magnesi B6 Stella Trị Đau Đầu'
            END
        WHEN 11 THEN -- Tieu duong noi tiet
            CASE (@m % 4)
                WHEN 0 THEN N'Glucophage 850mg (Metformin)'
                WHEN 1 THEN N'Januvia 100mg (Sitagliptin)'
                WHEN 2 THEN N'Diamicron MR 60mg (Gliclazide)'
                ELSE N'Forxiga 10mg (Dapagliflozin)'
            END
        WHEN 12 THEN -- Xuong khop
            CASE (@m % 4)
                WHEN 0 THEN N'Glucosamine 500mg Dược Hậu Giang'
                WHEN 1 THEN N'Arcoxia 90mg (Etoricoxib)'
                WHEN 2 THEN N'Celebrex 200mg (Celecoxib)'
                ELSE N'Viartril-S 500mg Gói Uống'
            END
        WHEN 13 THEN -- Dong y thao duoc
            CASE (@m % 4)
                WHEN 0 THEN N'Bảo Khang Hoạt Huyết Dưỡng Não'
                WHEN 1 THEN N'Boganic Traphaco Bổ Gan'
                WHEN 2 THEN N'Kim Tiền Thảo OPC Trị Sỏi'
                ELSE N'Dạ Hương Dung Dịch Vệ Sinh'
            END
        WHEN 14 THEN -- Duoc my pham
            CASE (@m % 4)
                WHEN 0 THEN N'Sữa Rửa Mặt Cetaphil Gentle Cleanser 250ml'
                WHEN 1 THEN N'Kem Dưỡng Ẩm La Roche-Posay B5 40ml'
                WHEN 2 THEN N'Kem Trị Mụn Klenzit-C 15g'
                ELSE N'Sữa Chống Nắng Anessa Perfect UV 60ml'
            END
        ELSE -- Vat tu y te
            CASE (@m % 4)
                WHEN 0 THEN N'Băng Cá Nhân Urgo Hộp 100 Miếng'
                WHEN 1 THEN N'Cồn Y Tế 70 Độ Chai 500ml'
                WHEN 2 THEN N'Khẩu Trang Y Tế 4 Lớp Kháng Khuẩn'
                ELSE N'Nước Muối Sinh Lý Natri Clorid 0.9% 500ml'
            END
    END + ' (SKU-' + RIGHT('000' + CAST(@m AS NVARCHAR), 4) + ')'

    IF @m_cat IN (8, 14) AND @m % 2 = 0
    BEGIN
        SET @m_base_u = 8  -- Tuyp
        SET @m_sale_u = 3  -- Hop
    END
    ELSE IF @m_cat IN (4, 5, 9, 15) AND @m % 3 = 0
    BEGIN
        SET @m_base_u = 15 -- ml
        SET @m_sale_u = 4  -- Chai
    END
    ELSE IF @m_cat IN (4, 5, 12) AND @m % 4 = 0
    BEGIN
        SET @m_base_u = 6  -- Goi
        SET @m_sale_u = 3  -- Hop
    END
    ELSE
    BEGIN
        SET @m_base_u = 1  -- Vien
        SET @m_sale_u = CASE (@m % 3) WHEN 0 THEN 2 WHEN 1 THEN 3 ELSE 5 END -- Vi, Hop, Lo
    END

    INSERT INTO [Medicine] ([MedicineName], [DefaultRetailPrice], [DefaultWholesalePrice], [VATPercent], [CategoryID], [ManufacturerID], [BaseUnitID], [MedicineCategoryID], [UnitID], [IsDeleted])
    VALUES (@m_name, @m_price, CAST(@m_price * 0.78 AS DECIMAL(18,2)), @m_vat, @m_cat, @m_manu, @m_base_u, @m_cat, @m_sale_u, 0)
    SET @m = @m + 1
END

-- =============================================================================
-- 17. UNIT CONVERSIONS (Quy doi don vi chinh xac theo tung thuoc)
-- =============================================================================
PRINT '17/32 - Dang tao UnitConversion...'
DECLARE @uc INT = 1
DECLARE @base_u BIGINT
WHILE @uc <= 400
BEGIN
    SELECT @base_u = [BaseUnitID] FROM [Medicine] WHERE [MedicineID] = @uc

    IF @base_u = 1 -- Vien
    BEGIN
        INSERT INTO [UnitConversion] ([MedicineID], [UnitID], [Factor]) VALUES
        (@uc, 2, 10.00),   -- 1 Vi = 10 Vien
        (@uc, 3, 100.00)   -- 1 Hop = 100 Vien
    END
    ELSE IF @base_u = 15 -- ml
    BEGIN
        INSERT INTO [UnitConversion] ([MedicineID], [UnitID], [Factor]) VALUES
        (@uc, 4, 100.00)   -- 1 Chai = 100 ml
    END
    ELSE IF @base_u = 6 -- Goi
    BEGIN
        INSERT INTO [UnitConversion] ([MedicineID], [UnitID], [Factor]) VALUES
        (@uc, 3, 24.00)    -- 1 Hop = 24 Goi
    END
    ELSE IF @base_u = 8 -- Tuyp
    BEGIN
        INSERT INTO [UnitConversion] ([MedicineID], [UnitID], [Factor]) VALUES
        (@uc, 3, 1.00)     -- 1 Hop = 1 Tuyp
    END
    SET @uc = @uc + 1
END

-- =============================================================================
-- 18. PRICE LIST ITEMS (Tat ca thuoc deu co gia trong bang gia chung)
-- =============================================================================
PRINT '18/32 - Dang tao PriceListItem...'
INSERT INTO [PriceListItem] ([PriceListID], [MedicineID], [RetailPrice], [WholesalePrice])
SELECT 1, [MedicineID], [DefaultRetailPrice], [DefaultWholesalePrice]
FROM [Medicine]

-- =============================================================================
-- 19. PROMOTIONS & PROMOTION ITEMS (Cac chuong trinh khuyen mai dang chay)
-- =============================================================================
PRINT '19/32 - Dang tao Promotion & PromotionItem...'
INSERT INTO [Promotion] ([PromotionName], [StartDate], [EndDate]) VALUES
(N'Tri Ân Khách Hàng - Giảm Giá Sức Khỏe Mùa Thu', DATEADD(month, -1, GETDATE()), DATEADD(month, 2, GETDATE())),
(N'Tháng Chăm Sóc Sức Khỏe Gia Đình', DATEADD(day, -15, GETDATE()), DATEADD(day, 45, GETDATE())),
(N'Khuyến Mãi Sản Phẩm Vitamin & Đề Kháng', DATEADD(day, -5, GETDATE()), DATEADD(month, 1, GETDATE()))

INSERT INTO [PromotionItem] ([PromotionID], [MedicineID], [DiscountValue]) VALUES
(1, 1, 5000.00), (1, 2, 8000.00), (1, 5, 10000.00), (1, 10, 15000.00),
(2, 15, 7000.00), (2, 20, 12000.00), (2, 25, 5000.00), (2, 30, 20000.00),
(3, 40, 10000.00), (3, 45, 15000.00), (3, 50, 8000.00), (3, 55, 25000.00)

-- =============================================================================
-- 20. GOODS RECEIPTS, ITEMS & BATCHES (400 phieu nhap, tao kho va lo thuoc)
-- =============================================================================
PRINT '20/32 - Dang tao GoodsReceipt, GoodsReceiptItem & Batch...'
DECLARE @gr INT = 1
DECLARE @gr_branch INT
DECLARE @gr_warehouse BIGINT
DECLARE @gr_user BIGINT
DECLARE @gr_sup BIGINT
DECLARE @gr_date DATETIME2
DECLARE @gr_id BIGINT
DECLARE @gr_total DECIMAL(18,2)
DECLARE @gr_status INT
DECLARE @gr_approved_by BIGINT
DECLARE @gr_approved_at DATETIME2
DECLARE @num_items INT
DECLARE @item_idx INT
DECLARE @med_id BIGINT
DECLARE @med_base_unit BIGINT
DECLARE @unit_cost DECIMAL(18,2)
DECLARE @u_name NVARCHAR(255)
DECLARE @qty DECIMAL(18,2)
DECLARE @gri_id BIGINT
DECLARE @mfg DATETIME2
DECLARE @exp DATETIME2
DECLARE @stock_qty DECIMAL(18,2)

WHILE @gr <= 400
BEGIN
    SET @gr_branch = ((@gr - 1) % 4) + 1
    SET @gr_warehouse = @gr_branch -- 1 kho tuong ung voi branch
    SET @gr_user = CASE @gr_branch WHEN 1 THEN 11 WHEN 2 THEN 12 WHEN 3 THEN 13 ELSE 14 END
    SET @gr_sup = ((@gr - 1) % 50) + 1
    SET @gr_date = DATEADD(day, - (400 - @gr), GETDATE())

    -- StatusTicket: 0=PENDING, 1=APPROVED, 2=COMPLETE, 3=REJECTED
    IF @gr <= 380
    BEGIN
        SET @gr_status = 2 -- COMPLETE (Da nhap kho)
        SET @gr_approved_by = 1
        SET @gr_approved_at = DATEADD(hour, 1, @gr_date)
    END
    ELSE IF @gr <= 390
    BEGIN
        SET @gr_status = 1 -- APPROVED (Da duyet)
        SET @gr_approved_by = 1
        SET @gr_approved_at = DATEADD(hour, 1, @gr_date)
    END
    ELSE IF @gr <= 398
    BEGIN
        SET @gr_status = 0 -- PENDING (Cho duyet)
        SET @gr_approved_by = NULL
        SET @gr_approved_at = NULL
    END
    ELSE
    BEGIN
        SET @gr_status = 3 -- REJECTED (Tu choi)
        SET @gr_approved_by = 1
        SET @gr_approved_at = DATEADD(hour, 1, @gr_date)
    END

    INSERT INTO [GoodsReceipt] ([ReceiptNumber], [SupplierID], [BranchID], [UserID], [ReceiptDate], [TotalAmount], [PaidAmount], [Note], [ApprovedBy], [ApprovedAt], [Status], [IsDeleted])
    VALUES (@gr, @gr_sup, @gr_branch, @gr_user, @gr_date, 0, 0, N'Phiếu nhập kho định kỳ số ' + CAST(@gr AS NVARCHAR), @gr_approved_by, @gr_approved_at, @gr_status, 0)
    SET @gr_id = SCOPE_IDENTITY()

    SET @gr_total = 0
    SET @num_items = (@gr % 3) + 2 -- 2 to 4 items
    SET @item_idx = 1

    WHILE @item_idx <= @num_items
    BEGIN
        SET @med_id = ((@gr * 7 + @item_idx * 13 - 1) % 400) + 1

        SELECT @med_base_unit = m.[BaseUnitID], @unit_cost = m.[DefaultWholesalePrice], @u_name = u.[UnitName]
        FROM [Medicine] m
        JOIN [Unit] u ON m.[BaseUnitID] = u.[UnitID]
        WHERE m.[MedicineID] = @med_id

        SET @qty = CAST(((@gr * 3 + @item_idx * 17) % 300) + 50 AS DECIMAL(18,2))

        INSERT INTO [GoodsReceiptItem] ([GoodsReceiptID], [MedicineID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [UnitCost])
        VALUES (@gr_id, @med_id, @med_base_unit, @u_name, @qty, @qty, @unit_cost)
        SET @gri_id = SCOPE_IDENTITY()

        -- Tao lo hang (Batch)
        SET @mfg = DATEADD(month, -6, @gr_date)

        IF @gr % 25 = 0 AND @item_idx = 1
            SET @exp = DATEADD(day, -10, GETDATE()) -- Da het han 10 ngay
        ELSE IF @gr % 15 = 0 AND @item_idx = 1
            SET @exp = DATEADD(month, 4, GETDATE())  -- Het han sau 4 thang (3-6 thang)
        ELSE
            SET @exp = DATEADD(year, 2, @gr_date)    -- Con han 2 nam

        SET @stock_qty = @qty
        IF @gr % 17 = 0 AND @item_idx = 1
            SET @stock_qty = CAST((@gr % 7) + 2 AS DECIMAL(18,2)) -- Ton 2-8 don vi (< 10)

        INSERT INTO [Batch] ([GoodsReceiptItemID], [WarehouseID], [QuantityReceived], [QuantityInStock], [ManufactureDate], [ExpiryDate], [Note])
        VALUES (@gri_id, @gr_warehouse, @qty, @stock_qty, @mfg, @exp, N'Lô nhập kho tự động ' + CAST(@gri_id AS NVARCHAR))

        SET @gr_total = @gr_total + (@qty * @unit_cost)
        SET @item_idx = @item_idx + 1
    END

    UPDATE [GoodsReceipt]
    SET [TotalAmount] = @gr_total, [PaidAmount] = CAST(@gr_total * 0.85 AS DECIMAL(18,2))
    WHERE [GoodsReceiptID] = @gr_id

    SET @gr = @gr + 1
END

-- =============================================================================
-- 21. INVOICES & INVOICE ITEMS (Customer 1 itsephu: CON NO; Customer 2 namphuits: HET NO)
-- =============================================================================
PRINT '21/32 - Dang tao Invoice & InvoiceItem...'

-- 1. Customer 1 (itsephu): Invoice 1 (3.000.000, da tra 1.000.000, con no 2.000.000 => PaymentStatus = 0)
INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [PaymentStatus], [Note], [IsDeleted])
VALUES (1, 1, 7, DATEADD(day, -25, GETDATE()), 3000000.00, 1000000.00, 0, N'Hóa đơn mua thuốc đợt 1 - itsephu (Còn nợ)', 0);
DECLARE @inv1 BIGINT = SCOPE_IDENTITY();
INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice]) VALUES
(@inv1, 1, 3, N'Hộp', 10.00, 100.00, 1000.00, 150000.00),
(@inv1, 2, 3, N'Hộp', 10.00, 100.00, 1000.00, 150000.00);
UPDATE [Batch] SET [QuantityInStock] = [QuantityInStock] - 10.00 WHERE [BatchID] IN (1, 2);

-- 2. Customer 1 (itsephu): Invoice 2 (2.500.000, chua tra => PaymentStatus = 0)
INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [PaymentStatus], [Note], [IsDeleted])
VALUES (1, 1, 7, DATEADD(day, -10, GETDATE()), 2500000.00, 0.00, 0, N'Hóa đơn mua thuốc đợt 2 - itsephu (Chưa thanh toán)', 0);
DECLARE @inv2 BIGINT = SCOPE_IDENTITY();
INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice]) VALUES
(@inv2, 3, 3, N'Hộp', 5.00, 100.00, 500.00, 500000.00);
UPDATE [Batch] SET [QuantityInStock] = [QuantityInStock] - 5.00 WHERE [BatchID] = 3;

-- 3. Customer 2 (namphuits): Invoice 3 (2.000.000, da tra du 2.000.000 => PaymentStatus = 1)
INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [PaymentStatus], [Note], [IsDeleted])
VALUES (2, 1, 7, DATEADD(day, -20, GETDATE()), 2000000.00, 2000000.00, 1, N'Hóa đơn mua thuốc theo đơn - namphuits (Đã thanh toán)', 0);
DECLARE @inv3 BIGINT = SCOPE_IDENTITY();
INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice]) VALUES
(@inv3, 4, 3, N'Hộp', 10.00, 100.00, 1000.00, 200000.00);
UPDATE [Batch] SET [QuantityInStock] = [QuantityInStock] - 10.00 WHERE [BatchID] = 4;

-- 4. Customer 2 (namphuits): Invoice 4 (1.500.000, da tra du 1.500.000 => PaymentStatus = 1)
INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [PaymentStatus], [Note], [IsDeleted])
VALUES (2, 1, 7, DATEADD(day, -8, GETDATE()), 1500000.00, 1500000.00, 1, N'Hóa đơn mua vitamin - namphuits (Đã thanh toán)', 0);
DECLARE @inv4 BIGINT = SCOPE_IDENTITY();
INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice]) VALUES
(@inv4, 5, 3, N'Hộp', 5.00, 100.00, 500.00, 300000.00);
UPDATE [Batch] SET [QuantityInStock] = [QuantityInStock] - 5.00 WHERE [BatchID] = 5;

-- Invoices 5..400 cho cac khach hang khac
DECLARE @inv INT = 5
DECLARE @inv_branch INT
DECLARE @inv_user BIGINT
DECLARE @inv_cust BIGINT
DECLARE @inv_date DATETIME2
DECLARE @inv_id BIGINT
DECLARE @inv_total DECIMAL(18,2)
DECLARE @inv_paid DECIMAL(18,2)
DECLARE @inv_status INT
DECLARE @inv_num_items INT
DECLARE @inv_item_idx INT
DECLARE @batch_id BIGINT
DECLARE @batch_stock DECIMAL(18,2)
DECLARE @item_price DECIMAL(18,2)
DECLARE @item_unit_id BIGINT
DECLARE @item_unit_name NVARCHAR(255)
DECLARE @sell_qty DECIMAL(18,2)

WHILE @inv <= 400
BEGIN
    SET @inv_branch = ((@inv - 1) % 4) + 1
    SET @inv_user = CASE @inv_branch WHEN 1 THEN 7 WHEN 2 THEN 8 WHEN 3 THEN 9 ELSE 10 END
    SET @inv_cust = ((@inv * 3) % 398) + 3 -- CustomerID tu 3 den 400

    IF @inv > 350
        SET @inv_date = DATEADD(minute, ((@inv * 23) % 720), DATEADD(day, - (400 - @inv) % 7, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)))
    ELSE
        SET @inv_date = DATEADD(day, - (400 - @inv), GETDATE())

    INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [PaymentStatus], [Note], [IsDeleted])
    VALUES (@inv_cust, @inv_branch, @inv_user, @inv_date, 0, 0, 0, N'Hóa đơn bán hàng điện tử số ' + CAST(@inv AS NVARCHAR), 0)
    SET @inv_id = SCOPE_IDENTITY()

    SET @inv_total = 0
    SET @inv_num_items = (@inv % 3) + 2
    SET @inv_item_idx = 1

    WHILE @inv_item_idx <= @inv_num_items
    BEGIN
        SET @batch_id = ((@inv * 5 + @inv_item_idx * 11) % 1000) + 1

        SELECT @batch_stock = b.[QuantityInStock], @item_price = m.[DefaultRetailPrice], @item_unit_id = m.[BaseUnitID], @item_unit_name = u.[UnitName]
        FROM [Batch] b
        JOIN [GoodsReceiptItem] gri ON b.[GoodsReceiptItemID] = gri.[GoodsReceiptItemID]
        JOIN [Medicine] m ON gri.[MedicineID] = m.[MedicineID]
        JOIN [Unit] u ON m.[BaseUnitID] = u.[UnitID]
        WHERE b.[BatchID] = @batch_id

        IF @batch_stock IS NULL OR @batch_stock <= 2
            SET @batch_stock = 100

        SET @sell_qty = CAST(((@inv + @inv_item_idx * 3) % 5) + 1 AS DECIMAL(18,2))
        IF @sell_qty > @batch_stock SET @sell_qty = 1

        INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [UnitName], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice])
        VALUES (@inv_id, @batch_id, @item_unit_id, @item_unit_name, @sell_qty, 1.00, @sell_qty, @item_price)

        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] - @sell_qty
        WHERE [BatchID] = @batch_id AND [QuantityInStock] >= @sell_qty

        SET @inv_total = @inv_total + (@sell_qty * @item_price)
        SET @inv_item_idx = @inv_item_idx + 1
    END

    IF @inv % 5 = 0
    BEGIN
        SET @inv_paid = CAST(@inv_total * 0.5 AS DECIMAL(18,2))
        SET @inv_status = 0 -- Debt
    END
    ELSE
    BEGIN
        SET @inv_paid = @inv_total
        SET @inv_status = 1 -- Paid
    END

    UPDATE [Invoice]
    SET [TotalAmount] = @inv_total, [PaidAmount] = @inv_paid, [PaymentStatus] = @inv_status
    WHERE [InvoiceID] = @inv_id

    SET @inv = @inv + 1
END

-- =============================================================================
-- 22. RECEIPTS & RECEIPT DETAILS (Phieu thu tien & gach no hoa don)
-- =============================================================================
PRINT '22/32 - Dang tao Receipt & ReceiptDetail...'

-- 1. Receipt 1 cho Customer 1 (itsephu): Tra 1.000.000 cho Invoice 1
INSERT INTO [Receipt] ([CustomerID], [BranchID], [UserID], [TotalAmount], [PaymentMethod], [CreatedDate], [IsDeleted])
VALUES (1, 1, 7, 1000000.00, 0, DATEADD(day, -24, GETDATE()), 0);
DECLARE @rc1 BIGINT = SCOPE_IDENTITY();
INSERT INTO [ReceiptDetail] ([ReceiptID], [InvoiceID], [AmountApplied])
VALUES (@rc1, 1, 1000000.00);

-- 2. Receipt 2 cho Customer 2 (namphuits): Tra du 2.000.000 cho Invoice 3
INSERT INTO [Receipt] ([CustomerID], [BranchID], [UserID], [TotalAmount], [PaymentMethod], [CreatedDate], [IsDeleted])
VALUES (2, 1, 7, 2000000.00, 0, DATEADD(day, -20, GETDATE()), 0);
DECLARE @rc2 BIGINT = SCOPE_IDENTITY();
INSERT INTO [ReceiptDetail] ([ReceiptID], [InvoiceID], [AmountApplied])
VALUES (@rc2, 3, 2000000.00);

-- 3. Receipt 3 cho Customer 2 (namphuits): Tra du 1.500.000 cho Invoice 4
INSERT INTO [Receipt] ([CustomerID], [BranchID], [UserID], [TotalAmount], [PaymentMethod], [CreatedDate], [IsDeleted])
VALUES (2, 1, 7, 1500000.00, 0, DATEADD(day, -8, GETDATE()), 0);
DECLARE @rc3 BIGINT = SCOPE_IDENTITY();
INSERT INTO [ReceiptDetail] ([ReceiptID], [InvoiceID], [AmountApplied])
VALUES (@rc3, 4, 1500000.00);

-- Receipts 4..200 cho cac khach hang khac
DECLARE @rc INT = 4
DECLARE @rc_cust BIGINT
DECLARE @rc_branch BIGINT
DECLARE @rc_user BIGINT
DECLARE @rc_amt DECIMAL(18,2)
DECLARE @rc_date DATETIME2
DECLARE @rc_id BIGINT
DECLARE @target_inv_id BIGINT
DECLARE @target_inv_paid DECIMAL(18,2)

WHILE @rc <= 200
BEGIN
    SET @rc_cust = ((@rc - 4) % 398) + 3
    SET @rc_branch = ((@rc - 1) % 4) + 1
    SET @rc_user = CASE @rc_branch WHEN 1 THEN 7 WHEN 2 THEN 8 WHEN 3 THEN 9 ELSE 10 END
    SET @rc_amt = CAST((@rc * 75000) % 3000000 + 200000 AS DECIMAL(18,2))
    SET @rc_date = DATEADD(day, - (200 - @rc), GETDATE())

    INSERT INTO [Receipt] ([CustomerID], [BranchID], [UserID], [TotalAmount], [PaymentMethod], [CreatedDate], [IsDeleted])
    VALUES (@rc_cust, @rc_branch, @rc_user, @rc_amt, 0, @rc_date, 0)
    SET @rc_id = SCOPE_IDENTITY()

    -- Tim 1 hoa don cua khach hang nay de gan vao ReceiptDetail
    SELECT TOP 1 @target_inv_id = [InvoiceID], @target_inv_paid = [PaidAmount]
    FROM [Invoice]
    WHERE [CustomerID] = @rc_cust
    ORDER BY [InvoiceID] ASC

    IF @target_inv_id IS NOT NULL AND @target_inv_paid > 0
    BEGIN
        INSERT INTO [ReceiptDetail] ([ReceiptID], [InvoiceID], [AmountApplied])
        VALUES (@rc_id, @target_inv_id, CASE WHEN @rc_amt <= @target_inv_paid THEN @rc_amt ELSE @target_inv_paid END)
    END

    SET @rc = @rc + 1
END

-- =============================================================================
-- 23. PURCHASE RETURNS (50 phieu tra hang nha cung cap)
-- =============================================================================
PRINT '23/32 - Dang tao PurchaseReturn & PurchaseReturnItem...'
DECLARE @pr INT = 1
DECLARE @pr_sup BIGINT
DECLARE @pr_user BIGINT
DECLARE @pr_date DATETIME2
DECLARE @pr_id BIGINT
DECLARE @pr_batch BIGINT
DECLARE @pr_qty DECIMAL(18,2)

WHILE @pr <= 50
BEGIN
    SET @pr_sup = ((@pr * 3) % 50) + 1
    SET @pr_user = 11 + ((@pr - 1) % 4)
    SET @pr_date = DATEADD(day, - (100 - @pr), GETDATE())

    INSERT INTO [PurchaseReturn] ([SupplierID], [UserID], [CreatedAt], [Note])
    VALUES (@pr_sup, @pr_user, @pr_date, N'Trả hàng lỗi sản xuất/hỏng bao bì số ' + CAST(@pr AS NVARCHAR))
    SET @pr_id = SCOPE_IDENTITY()

    SET @pr_batch = ((@pr * 7) % 800) + 1
    SET @pr_qty = CAST((@pr % 4) + 1 AS DECIMAL(18,2))

    INSERT INTO [PurchaseReturnItem] ([PurchaseReturnID], [BatchID], [Quantity])
    VALUES (@pr_id, @pr_batch, @pr_qty)

    UPDATE [Batch]
    SET [QuantityInStock] = [QuantityInStock] - @pr_qty
    WHERE [BatchID] = @pr_batch AND [QuantityInStock] >= @pr_qty

    SET @pr = @pr + 1
END

-- =============================================================================
-- 24. SALES RETURNS (50 phieu khach hang tra lai thuoc)
-- =============================================================================
PRINT '24/32 - Dang tao SalesReturn & SalesReturnItem...'
DECLARE @sr INT = 1
DECLARE @sr_cust BIGINT
DECLARE @sr_user BIGINT
DECLARE @sr_date DATETIME2
DECLARE @sr_id BIGINT
DECLARE @sr_batch BIGINT
DECLARE @sr_qty DECIMAL(18,2)

WHILE @sr <= 50
BEGIN
    SET @sr_cust = ((@sr * 5) % 398) + 3
    SET @sr_user = 7 + ((@sr - 1) % 4)
    SET @sr_date = DATEADD(day, - (80 - @sr), GETDATE())

    INSERT INTO [SalesReturn] ([CustomerID], [UserID], [CreatedAt], [Note])
    VALUES (@sr_cust, @sr_user, @sr_date, N'Khách đổi trả theo quy định 24h số ' + CAST(@sr AS NVARCHAR))
    SET @sr_id = SCOPE_IDENTITY()

    SET @sr_batch = ((@sr * 11) % 800) + 1
    SET @sr_qty = 1.00

    INSERT INTO [SalesReturnItem] ([SalesReturnID], [BatchID], [Quantity])
    VALUES (@sr_id, @sr_batch, @sr_qty)

    UPDATE [Batch]
    SET [QuantityInStock] = [QuantityInStock] + @sr_qty
    WHERE [BatchID] = @sr_batch

    SET @sr = @sr + 1
END

-- =============================================================================
-- 25. STOCK TAKE (60 phieu kiem ke kho dinh ky)
-- =============================================================================
PRINT '25/32 - Dang tao StockTake & StockTakeItem...'
DECLARE @st INT = 1
DECLARE @st_wh BIGINT
DECLARE @st_user BIGINT
DECLARE @st_date DATETIME2
DECLARE @st_status INT
DECLARE @st_approved_by BIGINT
DECLARE @st_approved_at DATETIME2
DECLARE @st_id BIGINT
DECLARE @st_item_idx INT
DECLARE @st_batch BIGINT
DECLARE @sys_q DECIMAL(18,2)
DECLARE @act_q DECIMAL(18,2)
DECLARE @st_adj_q DECIMAL(18,2)
DECLARE @st_des_q DECIMAL(18,2)

WHILE @st <= 60
BEGIN
    SET @st_wh = ((@st - 1) % 4) + 1
    SET @st_user = CASE @st_wh WHEN 1 THEN 11 WHEN 2 THEN 12 WHEN 3 THEN 13 ELSE 14 END
    SET @st_date = DATEADD(day, - (120 - @st * 2), GETDATE())

    -- StatusTicket: 0=PENDING, 1=APPROVED, 2=COMPLETE, 3=REJECTED
    IF @st <= 40
    BEGIN
        SET @st_status = 2 -- COMPLETE (Hoan thanh kiem ke, san sang dieu chinh / huy)
        SET @st_approved_by = 1
        SET @st_approved_at = DATEADD(hour, 4, @st_date)
    END
    ELSE IF @st <= 50
    BEGIN
        SET @st_status = 1 -- APPROVED (Da duyet)
        SET @st_approved_by = 1
        SET @st_approved_at = DATEADD(hour, 4, @st_date)
    END
    ELSE IF @st <= 58
    BEGIN
        SET @st_status = 0 -- PENDING (Cho duyet)
        SET @st_approved_by = NULL
        SET @st_approved_at = NULL
    END
    ELSE
    BEGIN
        SET @st_status = 3 -- REJECTED (Tu choi)
        SET @st_approved_by = 1
        SET @st_approved_at = DATEADD(hour, 4, @st_date)
    END

    INSERT INTO [StockTake] ([WarehouseID], [UserID], [CreatedAt], [Note], [Status], [ApprovedBy], [ApprovedAt], [IsDeleted])
    VALUES (@st_wh, @st_user, @st_date, N'Phiếu kiểm kê định kỳ kỳ ' + CAST(@st AS NVARCHAR), @st_status, @st_approved_by, @st_approved_at, 0)
    SET @st_id = SCOPE_IDENTITY()

    SET @st_item_idx = 1
    WHILE @st_item_idx <= 3
    BEGIN
        SET @st_batch = ((@st * 5 + @st_item_idx * 7) % 800) + 1
        SELECT @sys_q = [QuantityInStock] FROM [Batch] WHERE [BatchID] = @st_batch
        IF @sys_q IS NULL SET @sys_q = 50.00

        SET @act_q = @sys_q
        SET @st_adj_q = 0.00
        SET @st_des_q = 0.00

        IF @st BETWEEN 1 AND 20
        BEGIN
            SET @act_q = @sys_q + CASE WHEN @st_item_idx = 1 THEN 2.00 ELSE -2.00 END
            SET @st_adj_q = ABS(@act_q - @sys_q)
        END
        ELSE IF @st BETWEEN 21 AND 40
        BEGIN
            SET @act_q = @sys_q - 1.00
            SET @st_des_q = ABS(@act_q - @sys_q)
        END

        INSERT INTO [StockTakeItem] ([StockTakeID], [BatchID], [SystemQuantity], [ActualQuantity], [DifferenceQuantity], [AdjustQuantity], [DestroyQuantity])
        VALUES (@st_id, @st_batch, @sys_q, @act_q, @act_q - @sys_q, @st_adj_q, @st_des_q)

        SET @st_item_idx = @st_item_idx + 1
    END

    SET @st = @st + 1
END

-- =============================================================================
-- 26. STOCK ADJUSTMENT (20 phieu dieu chinh sau kiem ke, unique StockTakeID)
-- =============================================================================
PRINT '26/32 - Dang tao StockAdjustment & StockAdjustmentItem...'
DECLARE @sa INT = 1
DECLARE @sa_st_id BIGINT
DECLARE @sa_wh BIGINT
DECLARE @sa_user BIGINT
DECLARE @sa_date DATETIME2
DECLARE @sa_id BIGINT
DECLARE @sa_status INT
DECLARE @sa_approved_by BIGINT
DECLARE @sa_approved_at DATETIME2

WHILE @sa <= 20
BEGIN
    SET @sa_st_id = @sa -- Link 1-to-1 voi StockTake 1..20

    SELECT @sa_wh = [WarehouseID], @sa_user = [UserID], @sa_date = DATEADD(hour, 2, [CreatedAt])
    FROM [StockTake]
    WHERE [StockTakeID] = @sa_st_id

    -- StatusTicket: 0=PENDING, 1=APPROVED, 2=COMPLETE, 3=REJECTED
    IF @sa <= 16
    BEGIN
        SET @sa_status = 2 -- COMPLETE
        SET @sa_approved_by = 1
        SET @sa_approved_at = DATEADD(hour, 1, @sa_date)
    END
    ELSE IF @sa <= 18
    BEGIN
        SET @sa_status = 0 -- PENDING
        SET @sa_approved_by = NULL
        SET @sa_approved_at = NULL
    END
    ELSE IF @sa = 19
    BEGIN
        SET @sa_status = 1 -- APPROVED
        SET @sa_approved_by = 1
        SET @sa_approved_at = DATEADD(hour, 1, @sa_date)
    END
    ELSE
    BEGIN
        SET @sa_status = 3 -- REJECTED
        SET @sa_approved_by = 1
        SET @sa_approved_at = DATEADD(hour, 1, @sa_date)
    END

    INSERT INTO [StockAdjustment] ([WarehouseID], [UserID], [StockTakeID], [Note], [CreatedAt], [ApprovedBy], [ApprovedAt], [Status], [IsDeleted])
    VALUES (@sa_wh, @sa_user, @sa_st_id, N'Điều chỉnh cân bằng kho sau kiểm kê ' + CAST(@sa_st_id AS NVARCHAR), @sa_date, @sa_approved_by, @sa_approved_at, @sa_status, 0)
    SET @sa_id = SCOPE_IDENTITY()

    INSERT INTO [StockAdjustmentItem] ([StockAdjustmentID], [BatchID], [StockTakeItemID], [AdjustQuantity], [ReasonCode])
    SELECT @sa_id, sti.[BatchID], sti.[StockTakeItemID], sti.[DifferenceQuantity], N'REASON_STOCK_TAKE_DIFF'
    FROM [StockTakeItem] sti
    WHERE sti.[StockTakeID] = @sa_st_id

    IF @sa_status = 2
    BEGIN
        UPDATE b
        SET b.[QuantityInStock] = b.[QuantityInStock] + sti.[DifferenceQuantity]
        FROM [Batch] b
        JOIN [StockTakeItem] sti ON b.[BatchID] = sti.[BatchID]
        WHERE sti.[StockTakeID] = @sa_st_id
    END

    SET @sa = @sa + 1
END

-- =============================================================================
-- 27. DESTROY RECEIPTS (20 phieu tieu huy thuoc hong/het han, unique StockTakeID)
-- =============================================================================
PRINT '27/32 - Dang tao DestroyReceipt & DestroyReceiptItem...'
DECLARE @dr INT = 1
DECLARE @dr_st_id BIGINT
DECLARE @dr_wh BIGINT
DECLARE @dr_user BIGINT
DECLARE @dr_date DATETIME2
DECLARE @dr_id BIGINT
DECLARE @dr_status INT
DECLARE @dr_approved_by BIGINT
DECLARE @dr_approved_at DATETIME2

WHILE @dr <= 20
BEGIN
    SET @dr_st_id = @dr + 20 -- Link 1-to-1 voi StockTake 21..40

    SELECT @dr_wh = [WarehouseID], @dr_user = [UserID], @dr_date = DATEADD(hour, 3, [CreatedAt])
    FROM [StockTake]
    WHERE [StockTakeID] = @dr_st_id

    -- StatusTicket: 0=PENDING, 1=APPROVED, 2=COMPLETE, 3=REJECTED
    IF @dr <= 16
    BEGIN
        SET @dr_status = 2 -- COMPLETE
        SET @dr_approved_by = 1
        SET @dr_approved_at = DATEADD(hour, 2, @dr_date)
    END
    ELSE IF @dr <= 18
    BEGIN
        SET @dr_status = 0 -- PENDING
        SET @dr_approved_by = NULL
        SET @dr_approved_at = NULL
    END
    ELSE IF @dr = 19
    BEGIN
        SET @dr_status = 1 -- APPROVED
        SET @dr_approved_by = 1
        SET @dr_approved_at = DATEADD(hour, 2, @dr_date)
    END
    ELSE
    BEGIN
        SET @dr_status = 3 -- REJECTED
        SET @dr_approved_by = 1
        SET @dr_approved_at = DATEADD(hour, 2, @dr_date)
    END

    INSERT INTO [DestroyReceipt] ([WarehouseID], [UserID], [StockTakeID], [Note], [CreatedAt], [ApprovedBy], [ApprovedAt], [Status], [IsDeleted])
    VALUES (@dr_wh, @dr_user, @dr_st_id, N'Tiêu hủy thuốc hết hạn/hỏng chất lượng số ' + CAST(@dr AS NVARCHAR), @dr_date, @dr_approved_by, @dr_approved_at, @dr_status, 0)
    SET @dr_id = SCOPE_IDENTITY()

    INSERT INTO [DestroyReceiptItem] ([DestroyReceiptID], [BatchID], [StockTakeItemID], [Quantity], [UnitCost], [ReasonCode])
    SELECT @dr_id, sti.[BatchID], sti.[StockTakeItemID], ABS(sti.[DifferenceQuantity]), gri.[UnitCost], N'REASON_EXPIRED_DAMAGED'
    FROM [StockTakeItem] sti
    JOIN [Batch] b ON sti.[BatchID] = b.[BatchID]
    JOIN [GoodsReceiptItem] gri ON b.[GoodsReceiptItemID] = gri.[GoodsReceiptItemID]
    WHERE sti.[StockTakeID] = @dr_st_id

    IF @dr_status = 2
    BEGIN
        UPDATE b
        SET b.[QuantityInStock] = CASE WHEN b.[QuantityInStock] >= ABS(sti.[DifferenceQuantity]) THEN b.[QuantityInStock] - ABS(sti.[DifferenceQuantity]) ELSE 0 END
        FROM [Batch] b
        JOIN [StockTakeItem] sti ON b.[BatchID] = sti.[BatchID]
        WHERE sti.[StockTakeID] = @dr_st_id
    END

    SET @dr = @dr + 1
END

-- =============================================================================
-- 28. CUSTOMER WALLET HISTORY (Lich su nap / su dung vi khach hang)
-- =============================================================================
PRINT '28/32 - Dang tao CustomerWalletHistory...'
IF OBJECT_ID(N'[CustomerWalletHistory]', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        INSERT INTO [CustomerWalletHistory] ([CustomerID], [TransactionType], [Amount], [RefType], [RefId], [CreateDate]) VALUES
        (1, 0, 500000.00, 0, 1, DATEADD(day, -20, GETDATE())),
        (1, 1, 500000.00, 1, 1, DATEADD(day, -15, GETDATE())),
        (2, 0, 1000000.00, 0, 2, DATEADD(day, -18, GETDATE())),
        (2, 1, 1000000.00, 1, 3, DATEADD(day, -12, GETDATE()));
    ');
END

-- =============================================================================
-- 29. CHAT CONVERSATION & CHAT MESSAGES (Hoi dap AI duoc ly & quan ly)
-- =============================================================================
PRINT '29/32 - Dang tao ChatConversation & ChatMessage...'
IF OBJECT_ID(N'[ChatConversation]', N'U') IS NOT NULL AND OBJECT_ID(N'[ChatMessage]', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        DECLARE @c1 BIGINT, @c2 BIGINT, @c3 BIGINT;
        INSERT INTO [ChatConversation] ([UserID], [Title], [CreatedAt], [UpdatedAt], [IsArchived])
        VALUES (1, N''Hỏi về cách dùng thuốc Panadol Extra'', DATEADD(day, -2, GETDATE()), GETDATE(), 0);
        SET @c1 = SCOPE_IDENTITY();
        INSERT INTO [ChatMessage] ([ConversationID], [Role], [Content], [CreatedAt]) VALUES
        (@c1, ''user'', N''Panadol Extra uống liều lượng như thế nào cho người lớn bị đau đầu?'', DATEADD(minute, -30, GETDATE())),
        (@c1, ''assistant'', N''Người lớn và trẻ em từ 12 tuổi trở lên: Uống 1 - 2 viên mỗi 4 - 6 giờ khi cần thiết. Không uống quá 8 viên trong 24 giờ. Tránh dùng chung với các thuốc khác có chứa Paracetamol hoặc Caffeine.'', DATEADD(minute, -30, GETDATE())),
        (@c1, ''user'', N''Phụ nữ mang thai có dùng được Panadol Extra không?'', DATEADD(minute, -30, GETDATE())),
        (@c1, ''assistant'', N''Phụ nữ mang thai không nên tự ý dùng Panadol Extra vì có chứa Caffeine (65mg/viên). Nên tham khảo ý kiến bác sĩ hoặc ưu tiên dùng Paracetamol đơn chất liều thấp nhất có hiệu quả.'', DATEADD(minute, -30, GETDATE()));

        INSERT INTO [ChatConversation] ([UserID], [Title], [CreatedAt], [UpdatedAt], [IsArchived])
        VALUES (7, N''Kiểm tra tồn kho thuốc Augmentin 1g'', DATEADD(day, -2, GETDATE()), GETDATE(), 0);
        SET @c2 = SCOPE_IDENTITY();
        INSERT INTO [ChatMessage] ([ConversationID], [Role], [Content], [CreatedAt]) VALUES
        (@c2, ''user'', N''Kiểm tra giúp tôi tồn kho thuốc Augmentin 1g tại Chi nhánh Hà Nội còn bao nhiêu?'', DATEADD(minute, -30, GETDATE())),
        (@c2, ''assistant'', N''Tại Kho Tổng Hà Nội, thuốc Augmentin 1g hiện còn 2 lô với tổng tồn kho là 145 hộp. Lô gần nhất hạn sử dụng đến tháng 12 năm tới.'', DATEADD(minute, -30, GETDATE())),
        (@c2, ''user'', N''Cảm ơn bạn nhé!'', DATEADD(minute, -30, GETDATE())),
        (@c2, ''assistant'', N''Rất sẵn lòng hỗ trợ bạn! Nếu cần tạo phiếu xuất hoặc kiểm tra thêm thuốc nào khác, bạn cứ nhắn tôi nhé.'', DATEADD(minute, -30, GETDATE()));

        INSERT INTO [ChatConversation] ([UserID], [Title], [CreatedAt], [UpdatedAt], [IsArchived])
        VALUES (3, N''Báo cáo doanh thu chi nhánh hôm nay'', DATEADD(day, -2, GETDATE()), GETDATE(), 0);
        SET @c3 = SCOPE_IDENTITY();
        INSERT INTO [ChatMessage] ([ConversationID], [Role], [Content], [CreatedAt]) VALUES
        (@c3, ''user'', N''Tổng hợp nhanh tình hình doanh thu ca sáng hôm nay của chi nhánh.'', DATEADD(minute, -30, GETDATE())),
        (@c3, ''assistant'', N''Doanh thu ca sáng tại Chi nhánh Hà Nội đạt 18.450.000 VNĐ với 32 đơn hàng thành công. Thuốc bán chạy nhất trong ca là Panadol Extra và Berocca sủi.'', DATEADD(minute, -30, GETDATE())),
        (@c3, ''user'', N''Có mặt hàng nào dưới định mức tồn kho không?'', DATEADD(minute, -30, GETDATE())),
        (@c3, ''assistant'', N''Hiện tại có 2 mặt hàng dưới định mức an toàn (< 10 hộp): Nước nhỏ mắt Tobradex 5ml (còn 4 lọ) và Otrivin 0.1% (còn 6 lọ). Đề xuất tạo yêu cầu nhập bổ sung.'', DATEADD(minute, -30, GETDATE()));
    ');
END

-- =============================================================================
-- 30. INVENTORY TRANSACTIONS (Lich su giao dich kho)
-- =============================================================================
PRINT '30/32 - Dang tao InventoryTransaction...'
IF OBJECT_ID(N'[InventoryTransaction]', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        INSERT INTO [InventoryTransaction] ([BatchID], [UserID], [ActionBy], [QuantityChange], [ReferenceID], [ReferenceType], [TransactionType], [CreatedAt])
        SELECT TOP 100 b.[BatchID], 1, 1, b.[QuantityReceived], b.[GoodsReceiptItemID], 1, 1, DATEADD(day, -30, GETDATE())
        FROM [Batch] b;
    ');
END

-- =============================================================================
-- 31. AUDIT LOG & DAM BAO TON KHO KHONG AM (Integrity Check)
-- =============================================================================
PRINT '31/32 - Dang tao AuditLog va kiem tra ton kho...'
IF OBJECT_ID(N'[AuditLog]', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        INSERT INTO [AuditLog] ([UserID], [Date], [TableName], [Action], [OldValue], [NewValue]) VALUES
        (1, DATEADD(day, -10, GETDATE()), 1, 1, NULL, N''Thêm mới danh mục thuốc''),
        (1, DATEADD(day, -5, GETDATE()), 2, 2, N''{"IsActive": false}'', N''{"IsActive": true}''),
        (2, DATEADD(day, -1, GETDATE()), 3, 1, NULL, N''Tạo phiếu nhập hàng nhà cung cấp'');
    ');
END

UPDATE [Batch]
SET [QuantityInStock] = 0
WHERE [QuantityInStock] < 0

-- =============================================================================
-- 32. COMMIT & HOAN THANH
-- =============================================================================
COMMIT TRANSACTION
PRINT '32/32 - HOAN THANH! Toan bo du lieu mau da duoc tao thanh cong vao Database.'
GO

-- Kiem tra tong quan du lieu vua tao:
SELECT 'User' AS TableName, COUNT(*) AS TotalRecords FROM [User]
UNION ALL SELECT 'Role', COUNT(*) FROM [Role]
UNION ALL SELECT 'Permission', COUNT(*) FROM [Permission]
UNION ALL SELECT 'Branch', COUNT(*) FROM [Branch]
UNION ALL SELECT 'WareHouse', COUNT(*) FROM [WareHouse]
UNION ALL SELECT 'Customer', COUNT(*) FROM [Customer]
UNION ALL SELECT 'Supplier', COUNT(*) FROM [Supplier]
UNION ALL SELECT 'Medicine', COUNT(*) FROM [Medicine]
UNION ALL SELECT 'GoodsReceipt', COUNT(*) FROM [GoodsReceipt]
UNION ALL SELECT 'Batch', COUNT(*) FROM [Batch]
UNION ALL SELECT 'Invoice', COUNT(*) FROM [Invoice]
UNION ALL SELECT 'Receipt', COUNT(*) FROM [Receipt]
UNION ALL SELECT 'CustomerDebtSummary', COUNT(*) FROM [CustomerDebtSummary]
UNION ALL SELECT 'StockTake', COUNT(*) FROM [StockTake]
UNION ALL SELECT 'StockAdjustment', COUNT(*) FROM [StockAdjustment]
UNION ALL SELECT 'DestroyReceipt', COUNT(*) FROM [DestroyReceipt]
UNION ALL SELECT 'Promotion', COUNT(*) FROM [Promotion]
GO

IF OBJECT_ID(N'[ReceiptDetail]', N'U') IS NOT NULL
    EXEC(N'SELECT ''ReceiptDetail'' AS TableName, COUNT(*) AS TotalRecords FROM [ReceiptDetail]');
IF OBJECT_ID(N'[CustomerWalletHistory]', N'U') IS NOT NULL
    EXEC(N'SELECT ''CustomerWalletHistory'' AS TableName, COUNT(*) AS TotalRecords FROM [CustomerWalletHistory]');
IF OBJECT_ID(N'[ChatConversation]', N'U') IS NOT NULL
    EXEC(N'SELECT ''ChatConversation'' AS TableName, COUNT(*) AS TotalRecords FROM [ChatConversation]');
IF OBJECT_ID(N'[ChatMessage]', N'U') IS NOT NULL
    EXEC(N'SELECT ''ChatMessage'' AS TableName, COUNT(*) AS TotalRecords FROM [ChatMessage]');
GO

-- Kiem tra 2 customer dac biet (itsephu & namphuits):
SELECT c.CustomerID, c.CustomerName, c.Email, c.WalletBalance,
       ISNULL(SUM(i.TotalAmount), 0) AS TotalInvoiced,
       ISNULL(SUM(i.PaidAmount), 0) AS TotalPaid,
       ISNULL(cds.ClosingBalance, 0) AS LatestDebtBalance
FROM [Customer] c
LEFT JOIN [Invoice] i ON c.CustomerID = i.CustomerID
LEFT JOIN [CustomerDebtSummary] cds ON c.CustomerID = cds.CustomerID AND cds.Year = YEAR(GETDATE()) AND cds.Month = MONTH(GETDATE())
WHERE c.Email IN ('itsephu', 'namphuits')
GROUP BY c.CustomerID, c.CustomerName, c.Email, c.WalletBalance, cds.ClosingBalance;
GO