/*
===============================================================================
SAMPLE DATA SCRIPT - Pharmacy Management System
===============================================================================
So luong mau: ~400 ban ghi/bang chinh
Logic: Tong tien, ton kho, thoi gian dam bao dung
Bang duoc loai bo: ChatHistory, AuditLog, InventoryTransaction, Promotion, PromotionItem
Bang gia: 1 bang gia chung duy nhat cho tat ca chi nhanh
===============================================================================
*/

USE [PharmacySystem]
GO
SET NOCOUNT ON
SET XACT_ABORT ON
BEGIN TRANSACTION

-- =============================================================================
-- 0. XOA DU LIEU CU (reset tat ca bang)
-- =============================================================================
PRINT '0/27 - Dang xoa du lieu cu...'

-- Xoa con truoc, cha sau theo FK
DELETE FROM [DestroyReceiptItem]
DBCC CHECKIDENT ([DestroyReceiptItem], RESEED, 0)

DELETE FROM [DestroyReceipt]
DBCC CHECKIDENT ([DestroyReceipt], RESEED, 0)

DELETE FROM [StockAdjustmentItem]
DBCC CHECKIDENT ([StockAdjustmentItem], RESEED, 0)

DELETE FROM [StockAdjustment]
DBCC CHECKIDENT ([StockAdjustment], RESEED, 0)

DELETE FROM [StockTakeItem]
DBCC CHECKIDENT ([StockTakeItem], RESEED, 0)

DELETE FROM [StockTake]
DBCC CHECKIDENT ([StockTake], RESEED, 0)

DELETE FROM [SalesReturnItem]
DBCC CHECKIDENT ([SalesReturnItem], RESEED, 0)

DELETE FROM [SalesReturn]
DBCC CHECKIDENT ([SalesReturn], RESEED, 0)

DELETE FROM [PurchaseReturnItem]
DBCC CHECKIDENT ([PurchaseReturnItem], RESEED, 0)

DELETE FROM [PurchaseReturn]
DBCC CHECKIDENT ([PurchaseReturn], RESEED, 0)

DELETE FROM [InvoiceItem]
DBCC CHECKIDENT ([InvoiceItem], RESEED, 0)

DELETE FROM [Invoice]
DBCC CHECKIDENT ([Invoice], RESEED, 0)

DELETE FROM [Batch]
DBCC CHECKIDENT ([Batch], RESEED, 0)

DELETE FROM [GoodsReceiptItem]
DBCC CHECKIDENT ([GoodsReceiptItem], RESEED, 0)

DELETE FROM [GoodsReceipt]
DBCC CHECKIDENT ([GoodsReceipt], RESEED, 0)

DELETE FROM [PriceListItem]
DBCC CHECKIDENT ([PriceListItem], RESEED, 0)

DELETE FROM [UnitConversion]
DBCC CHECKIDENT ([UnitConversion], RESEED, 0)

DELETE FROM [Medicine]
DBCC CHECKIDENT ([Medicine], RESEED, 0)

DELETE FROM [MedicineCategory]
DBCC CHECKIDENT ([MedicineCategory], RESEED, 0)

DELETE FROM [ManuFacturer]
DBCC CHECKIDENT ([ManuFacturer], RESEED, 0)

DELETE FROM [Unit]
DBCC CHECKIDENT ([Unit], RESEED, 0)

DELETE FROM [Supplier]
DBCC CHECKIDENT ([Supplier], RESEED, 0)

DELETE FROM [Receipt]
DBCC CHECKIDENT ([Receipt], RESEED, 0)

DELETE FROM [CustomerDebtSummary]
DBCC CHECKIDENT ([CustomerDebtSummary], RESEED, 0)

DELETE FROM [Customer]
DBCC CHECKIDENT ([Customer], RESEED, 0)

DELETE FROM [CustomerType]
DBCC CHECKIDENT ([CustomerType], RESEED, 0)

DELETE FROM [WareHouse]
DBCC CHECKIDENT ([WareHouse], RESEED, 0)

DELETE FROM [UserBranch]
DBCC CHECKIDENT ([UserBranch], RESEED, 0)

DELETE FROM [User]
DBCC CHECKIDENT ([User], RESEED, 0)

DELETE FROM [Branch]
DBCC CHECKIDENT ([Branch], RESEED, 0)

DELETE FROM [PriceList]
DBCC CHECKIDENT ([PriceList], RESEED, 0)

DELETE FROM [RolePermission]
DBCC CHECKIDENT ([RolePermission], RESEED, 0)

DELETE FROM [Permission]
DBCC CHECKIDENT ([Permission], RESEED, 0)

DELETE FROM [Role]
DBCC CHECKIDENT ([Role], RESEED, 0)

-- =============================================================================
-- 1. ROLES (5 vai tro co dinh)
-- =============================================================================
PRINT '1/27 - Dang tao Roles...'
INSERT INTO [Role] ([RoleName]) VALUES
('admin'),
('manage_supply'),
('manage_branch'),
('user_sale'),
('user_warehouse')

-- =============================================================================
-- 1b. PERMISSIONS (Module_Action pattern)
-- =============================================================================
PRINT '1b/25 - Dang tao Permissions...'
INSERT INTO [Permission] ([Name], [Description]) VALUES
-- Medicine
('Medicine_View', N'Xem danh sach thuoc'),
('Medicine_Create', N'Them thuoc moi'),
('Medicine_Update', N'Cap nhat thong tin thuoc'),
('Medicine_Delete', N'Xoa thuoc'),
-- MedicineCategory
('MedicineCategory_View', N'Xem danh muc thuoc'),
('MedicineCategory_Create', N'Them danh muc thuoc'),
('MedicineCategory_Update', N'Cap nhat danh muc thuoc'),
('MedicineCategory_Delete', N'Xoa danh muc thuoc'),
-- ManuFacturer
('ManuFacturer_View', N'Xem nha san xuat'),
('ManuFacturer_Create', N'Them nha san xuat'),
('ManuFacturer_Update', N'Cap nhat nha san xuat'),
('ManuFacturer_Delete', N'Xoa nha san xuat'),
-- Unit
('Unit_View', N'Xem don vi tinh'),
('Unit_Create', N'Them don vi tinh'),
('Unit_Update', N'Cap nhat don vi tinh'),
('Unit_Delete', N'Xoa don vi tinh'),
-- UnitConversion
('UnitConversion_View', N'Xem quy doi don vi'),
('UnitConversion_Create', N'Them quy doi don vi'),
('UnitConversion_Update', N'Cap nhat quy doi don vi'),
('UnitConversion_Delete', N'Xoa quy doi don vi'),
-- Supplier
('Supplier_View', N'Xem nha cung cap'),
('Supplier_Create', N'Them nha cung cap'),
('Supplier_Update', N'Cap nhat nha cung cap'),
('Supplier_Delete', N'Xoa nha cung cap'),
-- Customer
('Customer_View', N'Xem khach hang'),
('Customer_Create', N'Them khach hang'),
('Customer_Update', N'Cap nhat khach hang'),
('Customer_Delete', N'Xoa khach hang'),
-- CustomerType
('CustomerType_View', N'Xem loai khach hang'),
('CustomerType_Create', N'Them loai khach hang'),
('CustomerType_Update', N'Cap nhat loai khach hang'),
('CustomerType_Delete', N'Xoa loai khach hang'),
-- Batch
('Batch_View', N'Xem lo hang'),
('Batch_Create', N'Them lo hang'),
('Batch_Update', N'Cap nhat lo hang'),
('Batch_Delete', N'Xoa lo hang'),
-- GoodsReceipt
('GoodsReceipt_View', N'Xem phieu nhap hang'),
('GoodsReceipt_Create', N'Tao phieu nhap hang'),
('GoodsReceipt_Update', N'Cap nhat phieu nhap hang'),
('GoodsReceipt_Delete', N'Xoa phieu nhap hang'),
-- Invoice
('Invoice_View', N'Xem hoa don ban hang'),
('Invoice_Create', N'Tao hoa don ban hang'),
('Invoice_Update', N'Cap nhat hoa don ban hang'),
('Invoice_Delete', N'Xoa hoa don ban hang'),
-- PurchaseReturn
('PurchaseReturn_View', N'Xem phieu tra hang nhap'),
('PurchaseReturn_Create', N'Tao phieu tra hang nhap'),
('PurchaseReturn_Update', N'Cap nhat phieu tra hang nhap'),
('PurchaseReturn_Delete', N'Xoa phieu tra hang nhap'),
-- SalesReturn
('SalesReturn_View', N'Xem phieu tra hang ban'),
('SalesReturn_Create', N'Tao phieu tra hang ban'),
('SalesReturn_Update', N'Cap nhat phieu tra hang ban'),
('SalesReturn_Delete', N'Xoa phieu tra hang ban'),
-- DestroyReceipt
('DestroyReceipt_View', N'Xem phieu tieu huy'),
('DestroyReceipt_Create', N'Tao phieu tieu huy'),
('DestroyReceipt_Update', N'Cap nhat phieu tieu huy'),
('DestroyReceipt_Delete', N'Xoa phieu tieu huy'),
-- StockTake
('StockTake_View', N'Xem phieu kiem ke'),
('StockTake_Create', N'Tao phieu kiem ke'),
('StockTake_Update', N'Cap nhat phieu kiem ke'),
('StockTake_Delete', N'Xoa phieu kiem ke'),
-- StockAdjustment
('StockAdjustment_View', N'Xem phieu dieu chinh kho'),
('StockAdjustment_Create', N'Tao phieu dieu chinh kho'),
('StockAdjustment_Update', N'Cap nhat phieu dieu chinh kho'),
('StockAdjustment_Delete', N'Xoa phieu dieu chinh kho'),
-- WareHouse
('WareHouse_View', N'Xem kho hang'),
('WareHouse_Create', N'Them kho hang'),
('WareHouse_Update', N'Cap nhat kho hang'),
('WareHouse_Delete', N'Xoa kho hang'),
-- PriceList
('PriceList_View', N'Xem bang gia'),
('PriceList_Create', N'Them bang gia'),
('PriceList_Update', N'Cap nhat bang gia'),
('PriceList_Delete', N'Xoa bang gia'),
-- Promotion
('Promotion_View', N'Xem chuong trinh khuyen mai'),
('Promotion_Create', N'Them chuong trinh khuyen mai'),
('Promotion_Update', N'Cap nhat chuong trinh khuyen mai'),
('Promotion_Delete', N'Xoa chuong trinh khuyen mai'),
-- Branch
('Branch_View', N'Xem chi nhanh'),
('Branch_Create', N'Them chi nhanh'),
('Branch_Update', N'Cap nhat chi nhanh'),
('Branch_Delete', N'Xoa chi nhanh'),
-- User
('User_View', N'Xem nguoi dung'),
('User_Create', N'Them nguoi dung'),
('User_Update', N'Cap nhat nguoi dung'),
('User_Delete', N'Xoa nguoi dung'),
-- Role
('Role_View', N'Xem vai tro'),
('Role_Create', N'Them vai tro'),
('Role_Update', N'Cap nhat vai tro'),
('Role_Delete', N'Xoa vai tro')

-- =============================================================================
-- 1c. ROLE PERMISSIONS (gan quyen cho tung vai tro)
-- =============================================================================
PRINT '1c/25 - Dang tao RolePermissions...'

-- admin: tat ca quyen
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 1, [PermissionID] FROM [Permission]

-- manage_supply: quyen nhap hang, nha cung cap, thuoc, kho, bang gia, khuyen mai
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 2, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'Medicine%'
   OR [Name] LIKE 'MedicineCategory%'
   OR [Name] LIKE 'ManuFacturer%'
   OR [Name] LIKE 'Unit%'
   OR [Name] LIKE 'UnitConversion%'
   OR [Name] LIKE 'Supplier%'
   OR [Name] LIKE 'GoodsReceipt%'
   OR [Name] LIKE 'PurchaseReturn%'
   OR [Name] LIKE 'Batch%'
   OR [Name] LIKE 'WareHouse%'
   OR [Name] LIKE 'PriceList%'
   OR [Name] LIKE 'Promotion%'
   OR [Name] LIKE 'StockTake%'
   OR [Name] LIKE 'StockAdjustment%'
   OR [Name] LIKE 'DestroyReceipt%'
   OR [Name] LIKE 'User_View'

-- manage_branch: quyen ban hang, khach hang, kho, kiem ke, dieu chinh
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 3, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'Invoice%'
   OR [Name] LIKE 'Customer%'
   OR [Name] LIKE 'CustomerType%'
   OR [Name] LIKE 'SalesReturn%'
   OR [Name] LIKE 'Batch%'
   OR [Name] LIKE 'WareHouse%'
   OR [Name] LIKE 'StockTake%'
   OR [Name] LIKE 'StockAdjustment%'
   OR [Name] LIKE 'DestroyReceipt%'
   OR [Name] LIKE 'Medicine_View'
   OR [Name] LIKE 'Unit_View'
   OR [Name] LIKE 'User_View'

-- user_sale: chi quyen ban hang, khach hang
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 4, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'Invoice%'
   OR [Name] LIKE 'Customer%'
   OR [Name] LIKE 'CustomerType%'
   OR [Name] LIKE 'SalesReturn%'
   OR [Name] LIKE 'Medicine_View'
   OR [Name] LIKE 'Unit_View'
   OR [Name] LIKE 'Batch_View'

-- user_warehouse: chi quyen nhap hang, kho, kiem ke
INSERT INTO [RolePermission] ([RoleID], [PermissionID])
SELECT 5, [PermissionID] FROM [Permission]
WHERE [Name] LIKE 'GoodsReceipt%'
   OR [Name] LIKE 'PurchaseReturn%'
   OR [Name] LIKE 'Batch%'
   OR [Name] LIKE 'WareHouse%'
   OR [Name] LIKE 'StockTake%'
   OR [Name] LIKE 'StockAdjustment%'
   OR [Name] LIKE 'DestroyReceipt%'
   OR [Name] LIKE 'Medicine_View'
   OR [Name] LIKE 'Unit_View'
   OR [Name] LIKE 'Supplier_View'

-- =============================================================================
-- 2. PRICELIST (1 bang gia chung)
-- =============================================================================
PRINT '2/27 - Dang tao PriceList...'
INSERT INTO [PriceList] ([PriceListName], [StartDate], [EndDate])
VALUES (N'Bang gia chung', '2024-01-01', '2025-12-31')

-- =============================================================================
-- 3. BRANCHES (4 chi nhanh - tat ca truoc PriceListID = 1)
-- =============================================================================
PRINT '3/27 - Dang tao Branches...'
INSERT INTO [Branch] ([BranchName], [Phone], [Address], [CreatedAt], [UpdatedAt], [IsActive], [PriceListID])
VALUES
(N'Chi nhanh Ha Noi', '0241234567', N'123 Le Loi, Quan Hoan Kiem, Ha Noi', '2023-01-01', '2024-01-01', 1, 1),
(N'Chi nhanh TP.HCM', '0281234567', N'456 Nguyen Hue, Quan 1, TP.HCM', '2023-01-01', '2024-01-01', 1, 1),
(N'Chi nhanh Da Nang', '0236123456', N'789 Tran Hung Dao, Quan Hai Chau, Da Nang', '2023-06-01', '2024-01-01', 1, 1),
(N'Chi nhanh Can Tho', '0292123456', N'321 Ly Tu Trong, Quan Ninh Kieu, Can Tho', '2023-06-01', '2024-01-01', 1, 1)

-- =============================================================================
-- 4. USERS (14 nguoi dung)
-- Role phan bo: admin(1), manage_supply(1), manage_branch(4), user_sale(4), user_warehouse(4)
-- =============================================================================
PRINT '4/27 - Dang tao Users...'
INSERT INTO [User] ([UserName], [PasswordHash], [FullName], [Phone], [Email], [IsActive], [FailedLoginCount])
VALUES
-- admin (1)
('user0001', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Nguyen Van Admin', '0900000001', 'user0001@pharmacy.vn', 1, 0),
-- manage_supply (1)
('user0002', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Tran Thi Supply', '0900000002', 'user0002@pharmacy.vn', 1, 0),
-- manage_branch (4: 1 per branch)
('user0003', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Le Minh Branch HaNoi', '0900000003', 'user0003@pharmacy.vn', 1, 0),
('user0004', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Pham Duc Branch HCM', '0900000004', 'user0004@pharmacy.vn', 1, 0),
('user0005', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Hoang Thi Branch DaNang', '0900000005', 'user0005@pharmacy.vn', 1, 0),
('user0006', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Phan Van Branch CanTho', '0900000006', 'user0006@pharmacy.vn', 1, 0),
-- user_sale (4: 1 per branch)
('user0007', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Vu Thi Sale HaNoi', '0900000007', 'user0007@pharmacy.vn', 1, 0),
('user0008', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Vo Minh Sale HCM', '0900000008', 'user0008@pharmacy.vn', 1, 0),
('user0009', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Dang Thi Sale DaNang', '0900000009', 'user0009@pharmacy.vn', 1, 0),
('user0010', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Bui Van Sale CanTho', '0900000010', 'user0010@pharmacy.vn', 1, 0),
-- user_warehouse (4: 1 per branch)
('user0011', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Do Thi Warehouse HaNoi', '0900000011', 'user0011@pharmacy.vn', 1, 0),
('user0012', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Ho Thi Warehouse HCM', '0900000012', 'user0012@pharmacy.vn', 1, 0),
('user0013', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Nguyen Minh Warehouse DaNang', '0900000013', 'user0013@pharmacy.vn', 1, 0),
('user0014', '$2a$11$aiv5b9gXTiPnJ5sygOp.oeHkbGkqv7cNinFQhshsQXyVnChK80ING', N'Tran Van Warehouse CanTho', '0900000014', 'user0014@pharmacy.vn', 1, 0)

DECLARE @i INT = 1

-- =============================================================================
-- 5. USERBRANCH
-- admin + manage_supply: gan tat ca 4 chi nhanh, IsDefault = 1
-- manage_branch + user_sale + user_warehouse: gan 1 chi nhanh, IsDefault = 1
-- =============================================================================
PRINT '5/27 - Dang tao UserBranch...'

-- admin (UserID 1) -> all branches, chi CN1 la default
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault])
SELECT 1, [BranchID], 1, CASE WHEN [BranchID] = 1 THEN 1 ELSE 0 END FROM [Branch]

-- manage_supply (UserID 2) -> all branches, chi CN1 la default
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault])
SELECT 2, [BranchID], 2, CASE WHEN [BranchID] = 1 THEN 1 ELSE 0 END FROM [Branch]

-- manage_branch -> 1 branch each (UserID 3->CN1, 4->CN2, 5->CN3, 6->CN4)
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault])
VALUES
(3, 1, 3, 1), (4, 2, 3, 1), (5, 3, 3, 1), (6, 4, 3, 1)

-- user_sale -> 1 branch each (UserID 7->CN1, 8->CN2, 9->CN3, 10->CN4)
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault])
VALUES
(7, 1, 4, 1), (8, 2, 4, 1), (9, 3, 4, 1), (10, 4, 4, 1)

-- user_warehouse -> 1 branch each (UserID 11->CN1, 12->CN2, 13->CN3, 14->CN4)
INSERT INTO [UserBranch] ([UserID], [BranchID], [RoleID], [IsDefault])
VALUES
(11, 1, 5, 1), (12, 2, 5, 1), (13, 3, 5, 1), (14, 4, 5, 1)

-- =============================================================================
-- 6. WAREHOUSES (1 kho chinh moi chi nhanh)
-- =============================================================================
PRINT '6/27 - Dang tao Warehouses...'
INSERT INTO [WareHouse] ([BranchID], [WarehouseName], [WarehouseType])
VALUES
(1, N'Kho chinh - Ha Noi', 1),
(2, N'Kho chinh - TP.HCM', 1),
(3, N'Kho chinh - Da Nang', 1),
(4, N'Kho chinh - Can Tho', 1)

-- =============================================================================
-- 7. CUSTOMER TYPES (3 loai)
-- =============================================================================
PRINT '7/27 - Dang tao CustomerType...'
INSERT INTO [CustomerType] ([TypeName], [DiscountPercent])
VALUES
(N'VIP', 5.00),
(N'Khach hang than thiet', 2.00),
(N'Khach hang thuong', 0.00)

-- =============================================================================
-- 8. CUSTOMERS (400 khach hang)
-- =============================================================================
PRINT '8/27 - Dang tao Customers...'
SET @i = 1
WHILE @i <= 400
BEGIN
    DECLARE @cust_name NVARCHAR(255) = CASE (@i % 15)
        WHEN 0 THEN N'Nguyen Van Tung' WHEN 1 THEN N'Tran Thi Mai' WHEN 2 THEN N'Le Hoang Nam'
        WHEN 3 THEN N'Pham Thanh Tam' WHEN 4 THEN N'Hoang Duc Long' WHEN 5 THEN N'Vo Thi Hue'
        WHEN 6 THEN N'Phan Minh Dat' WHEN 7 THEN N'Dang Van Khoa' WHEN 8 THEN N'Bui Thi Linh'
        WHEN 9 THEN N'Do Quoc Bao' WHEN 10 THEN N'Nguyen Thi Hanh' WHEN 11 THEN N'Tran Van Phuoc'
        WHEN 12 THEN N'Le Thanh Nhan' WHEN 13 THEN N'Pham Thi Kieu' WHEN 14 THEN N'Vo Van Duc'
    END + ' ' + CAST(@i AS NVARCHAR(10))

    INSERT INTO [Customer] ([CustomerName], [Phone], [Address], [CustomerTypeID])
    VALUES (
        @cust_name,
        '09' + RIGHT('00000000' + CAST((@i * 65537 % 100000000) AS NVARCHAR), 8),
        CAST(@i AS NVARCHAR) + N' Duong Le Loi, Quan ' + CAST((@i % 10) + 1 AS NVARCHAR) + N', TP.HCM',
        CASE (@i % 3) WHEN 0 THEN 1 WHEN 1 THEN 2 ELSE 3 END
    )
    SET @i = @i + 1
END

-- =============================================================================
-- 9. CUSTOMER DEBT SUMMARY (mot so khach hang co no)
-- =============================================================================
PRINT '9/27 - Dang tao CustomerDebtSummary...'
SET @i = 1
WHILE @i <= 100
BEGIN
    DECLARE @open_bal DECIMAL(18,2) = CAST((@i * 150000) % 5000000 AS DECIMAL(18,2))
    DECLARE @increase DECIMAL(18,2) = CAST((@i * 200000) % 3000000 AS DECIMAL(18,2))
    DECLARE @paid DECIMAL(18,2) = CAST((@i * 180000) % 4000000 AS DECIMAL(18,2))

    INSERT INTO [CustomerDebtSummary] ([Year], [Month], [OpeningBalance], [Increase], [Paid], [ClosingBalance], [IsLocked], [CustomerID])
    VALUES (
        '2024-01-01', '2024-01-01',
        @open_bal, @increase, @paid,
        @open_bal + @increase - @paid,
        0, @i
    )
    SET @i = @i + 1
END

-- =============================================================================
-- 10. RECEIPTS (phieu thu tien tu khach hang)
-- =============================================================================
PRINT '10/27 - Dang tao Receipts...'
SET @i = 1
WHILE @i <= 200
BEGIN
    INSERT INTO [Receipt] ([ReceiptNumber], [CustomerID], [TotalAmount])
    VALUES (@i, ((@i - 1) % 400) + 1, CAST((@i * 50000) % 10000000 + 100000 AS DECIMAL(18,2)))
    SET @i = @i + 1
END

-- =============================================================================
-- 11. SUPPLIERS (400 nha cung cap)
-- =============================================================================
PRINT '11/27 - Dang tao Suppliers...'
SET @i = 1
WHILE @i <= 400
BEGIN
    INSERT INTO [Supplier] ([SupplierName], [Phone], [Email], [Address])
    VALUES (
        N'Nha cung cap ' + CASE (@i % 10)
            WHEN 0 THEN N'Ha Noi Pharma' WHEN 1 THEN N'SaiGon Drug' WHEN 2 THEN N'Dong A Pharma'
            WHEN 3 THEN N'DHG Pharma' WHEN 4 THEN N'Domesco' WHEN 5 THEN N'Imexpharm'
            WHEN 6 THEN N'Mediplantex' WHEN 7 THEN N'Khanh Hoi Pharma' WHEN 8 THEN N'Hau Giang Pharma'
            WHEN 9 THEN N'Nha Trang Pharma'
        END + ' - ' + CAST(@i AS NVARCHAR(10)),
        '024' + RIGHT('00000000' + CAST((@i * 32771 % 100000000) AS NVARCHAR), 7),
        'contact' + CAST(@i AS NVARCHAR(10)) + '@supplier.vn',
        N'KCN ' + CAST((@i % 20) + 1 AS NVARCHAR) + N', Viet Nam'
    )
    SET @i = @i + 1
END

-- =============================================================================
-- 12. REFERENCE TABLES
-- =============================================================================
PRINT '12/27 - Dang tao MedicineCategory, Manufacturer, Unit...'

-- MedicineCategory (~15)
INSERT INTO [MedicineCategory] ([CategoryName])
VALUES
(N'Thuoc giam dau'), (N'Thuoc khang sinh'), (N'Thuoc ha sot'), (N'Thuoc tieu hoa'),
(N'Thuoc tim mach'), (N'Thuoc ho hap'), (N'Thuoc than kinh'), (N'Thuoc da lieu'),
(N'Thuoc mat'), (N'Thuoc tai mui hong'), (N'Vitamin & Khoang chat'), (N'Thuot di ung'),
(N'Thuoc noi tiet'), (N'Thuoc ung thu'), (N'Thuoc y hoc co truyen')

-- Manufacturer (~15)
INSERT INTO [ManuFacturer] ([ManufacturerName])
VALUES
(N'Cong ty Duoc Hau Giang'), (N'Cong ty Duoc Sai Gon'), (N'Cong ty Duoc Ha Noi'),
(N'Cong ty Duoc Trung uong'), (N'Cong ty Duoc Imexpharm'), (N'Cong ty Duoc DHG Pharma'),
(N'Cong ty Duoc Domesco'), (N'Cong ty Duoc Stellapharm'), (N'Cong ty Duoc Euvipharm'),
(N'Cong ty Duoc MeDi Pharma'), (N'Cong ty Duoc Sanofi Viet Nam'), (N'Cong ty Duoc Abbott'),
(N'Cong ty Duoc Pfizer Viet Nam'), (N'Cong ty Duoc Opsonin'), (N'Cong ty Duoc Bluedot')

-- Unit (~15)
INSERT INTO [Unit] ([UnitName])
VALUES
(N'Vien'), (N'Vi'), (N'Hop'), (N'Chai'), (N'Lo'), (N'Goi'), (N'Ong'),
(N'Tuyp'), (N'Bich'), (N'Thung'), (N'Tui'), (N'Chieu'), (N'Gam'),
(N'mg'), (N'ml')

-- =============================================================================
-- 13. MEDICINES (400 thuoc)
-- BaseUnitID: don vi nho nhat (Vien=1, ml=15, Tuyp=8, Gam=13, Chai=4)
-- UnitID: don vi ban le, LUON KHAC BaseUnitID
-- =============================================================================
PRINT '13/27 - Dang tao Medicines...'
SET @i = 1
WHILE @i <= 400
BEGIN
    DECLARE @med_name NVARCHAR(255)
    DECLARE @base_price DECIMAL(18,2) = CAST((@i * 12347 % 500000) + 5000 AS DECIMAL(18,2))

    SET @med_name = CASE (@i % 20)
        WHEN 0 THEN N'Paracetamol' WHEN 1 THEN N'Amoxicillin' WHEN 2 THEN N'Ibuprofen'
        WHEN 3 THEN N'Cetirizine' WHEN 4 THEN N'Omeprazole' WHEN 5 THEN N'Metformin'
        WHEN 6 THEN N'Atorvastatin' WHEN 7 THEN N'Amlodipine' WHEN 8 THEN N'Losartan'
        WHEN 9 THEN N'Azithromycin' WHEN 10 THEN N'Cephalexin' WHEN 11 THEN N'Diclofenac'
        WHEN 12 THEN N'Salbutamol' WHEN 13 THEN N'Loratadine' WHEN 14 THEN N'Pantoprazole'
        WHEN 15 THEN N'Simvastatin' WHEN 16 THEN N'Metoprolol' WHEN 17 THEN N'Valsartan'
        WHEN 18 THEN N'Clarithromycin' WHEN 19 THEN N'Levofloxacin'
    END + ' ' + CASE (@i % 6)
        WHEN 0 THEN '500mg' WHEN 1 THEN '100mg' WHEN 2 THEN '200mg'
        WHEN 3 THEN '50mg' WHEN 4 THEN '10mg' WHEN 5 THEN '250mg'
    END + ' (' + CAST(@i AS NVARCHAR(10)) + ')'

    DECLARE @med_baseuid BIGINT = CASE (@i % 20)
        WHEN 0 THEN 1  WHEN 1 THEN 15 WHEN 2 THEN 1  WHEN 3 THEN 15 WHEN 4 THEN 1
        WHEN 5 THEN 15 WHEN 6 THEN 1  WHEN 7 THEN 8  WHEN 8 THEN 1  WHEN 9 THEN 8
        WHEN 10 THEN 1 WHEN 11 THEN 13 WHEN 12 THEN 1 WHEN 13 THEN 13 WHEN 14 THEN 1
        WHEN 15 THEN 4 WHEN 16 THEN 1 WHEN 17 THEN 1 WHEN 18 THEN 1 WHEN 19 THEN 1
    END

    DECLARE @med_unitid BIGINT = CASE (@i % 20)
        WHEN 0 THEN 3  WHEN 1 THEN 4  WHEN 2 THEN 3  WHEN 3 THEN 4  WHEN 4 THEN 3
        WHEN 5 THEN 4  WHEN 6 THEN 3  WHEN 7 THEN 5  WHEN 8 THEN 3  WHEN 9 THEN 5
        WHEN 10 THEN 3 WHEN 11 THEN 6 WHEN 12 THEN 3 WHEN 13 THEN 6 WHEN 14 THEN 3
        WHEN 15 THEN 10 WHEN 16 THEN 3 WHEN 17 THEN 3 WHEN 18 THEN 3 WHEN 19 THEN 3
    END

    INSERT INTO [Medicine] ([MedicineName], [DefaultRetailPrice], [DefaultWholesalePrice], [VATPercent], [CategoryID], [ManufacturerID], [BaseUnitID], [MedicineCategoryID], [UnitID])
    VALUES (
        @med_name,
        @base_price,
        CAST(@base_price * 0.8 AS DECIMAL(18,2)),
        CASE (@i % 3) WHEN 0 THEN 0 WHEN 1 THEN 5 ELSE 10 END,
        ((@i - 1) % 15) + 1,
        ((@i - 1) % 15) + 1,
        @med_baseuid,
        ((@i - 1) % 15) + 1,
        @med_unitid
    )
    SET @i = @i + 1
END

-- =============================================================================
-- 14. UNIT CONVERSION (quy doi don vi)
-- UnitID PHAI KHAC BaseUnitID cua Medicine tuong ung
-- =============================================================================
PRINT '14/27 - Dang tao UnitConversion...'
SET @i = 1
WHILE @i <= 100
BEGIN
    DECLARE @med_id_uc BIGINT = ((@i - 1) % 400) + 1
    DECLARE @baseuid BIGINT
    SELECT @baseuid = [BaseUnitID] FROM [Medicine] WHERE [MedicineID] = @med_id_uc

    DECLARE @uc_unitid BIGINT
    DECLARE @uc_factor DECIMAL(18,2)

    IF @baseuid = 1
    BEGIN
        IF @i % 2 = 0
        BEGIN SET @uc_unitid = 2 SET @uc_factor = 10 END
        ELSE
        BEGIN SET @uc_unitid = 3 SET @uc_factor = 100 END
    END
    ELSE IF @baseuid = 15
    BEGIN
        SET @uc_unitid = 4
        SET @uc_factor = 100
    END
    ELSE IF @baseuid = 8
    BEGIN
        SET @uc_unitid = 5
        SET @uc_factor = 24
    END
    ELSE IF @baseuid = 13
    BEGIN
        SET @uc_unitid = 6
        SET @uc_factor = 50
    END
    ELSE IF @baseuid = 4
    BEGIN
        SET @uc_unitid = 10
        SET @uc_factor = 12
    END
    ELSE
    BEGIN
        SET @uc_unitid = 3
        SET @uc_factor = 10
    END

    INSERT INTO [UnitConversion] ([UnitID], [MedicineID], [Factor])
    VALUES (@uc_unitid, @med_id_uc, @uc_factor)

    SET @i = @i + 1
END

-- =============================================================================
-- 15. PRICE LIST ITEMS (tat ca 400 thuoc deu co gia trong bang gia chung)
-- =============================================================================
PRINT '15/27 - Dang tao PriceListItem...'
INSERT INTO [PriceListItem] ([PriceListID], [MedicineID], [RetailPrice], [WholesalePrice])
SELECT 1, [MedicineID], [DefaultRetailPrice], [DefaultWholesalePrice]
FROM [Medicine]

-- =============================================================================
-- 16. GOODS RECEIPT (400 phieu nhap)
-- =============================================================================
PRINT '16/27 - Dang tao GoodsReceipt...'
SET @i = 1
WHILE @i <= 400
BEGIN
    DECLARE @gr_total DECIMAL(18,2) = 0
    DECLARE @gr_paid DECIMAL(18,2) = 0
    DECLARE @gr_id BIGINT
    DECLARE @branch_gr INT = ((@i - 1) % 4) + 1
    DECLARE @user_gr INT

    -- Chon user warehouse tai chi nhanh tuong ung
    SET @user_gr = CASE @branch_gr
        WHEN 1 THEN 11
        WHEN 2 THEN 12
        WHEN 3 THEN 13
        WHEN 4 THEN 14
    END

    INSERT INTO [GoodsReceipt] ([ReceiptNumber], [SupplierID], [BranchID], [UserID], [ReceiptDate], [TotalAmount], [PaidAmount], [Note])
    VALUES (
        @i,
        ((@i - 1) % 400) + 1,
        @branch_gr,
        @user_gr,
        DATEADD(DAY, ((@i - 1) % 730), '2024-01-01'),
        0, 0,
        N'Phieu nhap so ' + CAST(@i AS NVARCHAR(10))
    )
    SET @gr_id = SCOPE_IDENTITY()

    -- Tao 2-4 chi tiet moi phieu nhap
    DECLARE @item_count INT = (@i % 3) + 2
    DECLARE @j INT = 1
    WHILE @j <= @item_count
    BEGIN
        DECLARE @med_id_gr BIGINT = ((@i * 7 + @j * 13 - 1) % 400) + 1
        DECLARE @qty_gr DECIMAL(18,2) = CAST(((@i * 3 + @j * 7) % 200) + 20 AS DECIMAL(18,2))
        DECLARE @cost_gr DECIMAL(18,2)
        SELECT @cost_gr = [DefaultWholesalePrice] FROM [Medicine] WHERE [MedicineID] = @med_id_gr
        DECLARE @unit_cost_gr DECIMAL(18,2) = CAST(@cost_gr * (0.85 + (@j * 0.05)) AS DECIMAL(18,2))

        INSERT INTO [GoodsReceiptItem] ([GoodsReceiptID], [MedicineID], [UnitID], [Quantity], [ConversionFactor], [UnitCost])
        VALUES (@gr_id, @med_id_gr, ((@j - 1) % 5) + 1, @qty_gr, 1.0, @unit_cost_gr)

        DECLARE @gri_id BIGINT = SCOPE_IDENTITY()

        -- Tao Batch tu GoodsReceiptItem
        DECLARE @wh_id BIGINT = @branch_gr
        INSERT INTO [Batch] ([BatchNumber], [GoodsReceiptItemID], [WarehouseID], [QuantityReceived], [QuantityInStock], [ManufactureDate], [ExpiryDate], [Note])
        VALUES (
            @i * 100 + @j,
            @gri_id,
            @wh_id,
            @qty_gr,
            @qty_gr,
            DATEADD(MONTH, -6, DATEADD(DAY, ((@i + @j) % 30), '2024-01-01')),
            DATEADD(YEAR, 2, DATEADD(DAY, ((@i + @j) % 30), '2024-01-01')),
            N'Lo nhap tu phieu ' + CAST(@i AS NVARCHAR(10))
        )

        SET @gr_total = @gr_total + (@qty_gr * @unit_cost_gr)
        SET @j = @j + 1
    END

    -- Cap nhat tong tien phieu nhap
    SET @gr_paid = CAST(@gr_total * (0.6 + ((@i % 5) * 0.1)) AS DECIMAL(18,2))
    UPDATE [GoodsReceipt]
    SET [TotalAmount] = @gr_total, [PaidAmount] = @gr_paid
    WHERE [GoodsReceiptID] = @gr_id

    SET @i = @i + 1
END

-- =============================================================================
-- 17. INVOICES (400 hoa don ban)
-- =============================================================================
PRINT '17/27 - Dang tao Invoices...'
SET @i = 1
WHILE @i <= 400
BEGIN
    DECLARE @inv_total DECIMAL(18,2) = 0
    DECLARE @inv_id BIGINT
    DECLARE @branch_inv INT = ((@i - 1) % 4) + 1
    DECLARE @user_inv INT

    -- Chon user sale tai chi nhanh tuong ung
    SET @user_inv = CASE @branch_inv
        WHEN 1 THEN 7
        WHEN 2 THEN 8
        WHEN 3 THEN 9
        WHEN 4 THEN 10
    END

    INSERT INTO [Invoice] ([CustomerID], [BranchID], [UserID], [CreatedAt], [TotalAmount], [PaidAmount], [Note])
    VALUES (
        ((@i - 1) % 400) + 1,
        @branch_inv,
        @user_inv,
        DATEADD(DAY, ((@i - 1) % 730), '2024-01-01'),
        0, 0,
        N'Hoa don so ' + CAST(@i AS NVARCHAR(10))
    )
    SET @inv_id = SCOPE_IDENTITY()

    -- Tao 2-4 chi tiet moi hoa don
    DECLARE @inv_item_count INT = (@i % 3) + 2
    DECLARE @k INT = 1
    WHILE @k <= @inv_item_count
    BEGIN
        DECLARE @batch_id_inv BIGINT = ((@i * 5 + @k * 11) % 1200) + 1
        DECLARE @qty_inv DECIMAL(18,2) = CAST(((@i * 2 + @k * 3) % 30) + 1 AS DECIMAL(18,2))
        DECLARE @unit_price_inv DECIMAL(18,2)
        DECLARE @med_id_inv BIGINT

        -- Lay gia tu Batch -> GoodsReceiptItem -> Medicine
        SELECT @med_id_inv = gri.[MedicineID]
        FROM [GoodsReceiptItem] gri
        INNER JOIN [Batch] b ON b.[GoodsReceiptItemID] = gri.[GoodsReceiptItemID]
        WHERE b.[BatchID] = @batch_id_inv

        SELECT @unit_price_inv = [DefaultRetailPrice] FROM [Medicine] WHERE [MedicineID] = @med_id_inv

        IF @unit_price_inv IS NULL SET @unit_price_inv = 50000

        INSERT INTO [InvoiceItem] ([InvoiceID], [BatchID], [UnitID], [Quantity], [ConversionFactor], [BaseQuantity], [UnitPrice])
        VALUES (@inv_id, @batch_id_inv, 1, @qty_inv, 1.0, @qty_inv, @unit_price_inv)

        -- Cap nhat so luong ton trong Batch
        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] - @qty_inv
        WHERE [BatchID] = @batch_id_inv AND [QuantityInStock] >= @qty_inv

        SET @inv_total = @inv_total + (@qty_inv * @unit_price_inv)
        SET @k = @k + 1
    END

    -- Cap nhat tong tien hoa don
    DECLARE @inv_paid DECIMAL(18,2) = CAST(@inv_total * (0.7 + ((@i % 4) * 0.1)) AS DECIMAL(18,2))
    UPDATE [Invoice]
    SET [TotalAmount] = @inv_total, [PaidAmount] = @inv_paid
    WHERE [InvoiceID] = @inv_id

    SET @i = @i + 1
END

-- =============================================================================
-- 18. PURCHASE RETURNS (50 phieu tra nha cung cap)
-- =============================================================================
PRINT '18/27 - Dang tao PurchaseReturn...'
SET @i = 1
WHILE @i <= 50
BEGIN
    DECLARE @pr_id BIGINT
    INSERT INTO [PurchaseReturn] ([SupplierID], [UserID], [CreatedAt], [Note])
    VALUES (
        ((@i - 1) % 400) + 1,
        11 + ((@i - 1) % 4),
        DATEADD(DAY, ((@i - 1) % 365), '2024-06-01'),
        N'Phieu tra NCC so ' + CAST(@i AS NVARCHAR(10))
    )
    SET @pr_id = SCOPE_IDENTITY()

    -- 2-3 chi tiet tra
    DECLARE @pr_item_count INT = (@i % 2) + 2
    DECLARE @m INT = 1
    WHILE @m <= @pr_item_count
    BEGIN
        DECLARE @batch_pr BIGINT = ((@i * 3 + @m * 7) % 1200) + 1
        DECLARE @qty_pr DECIMAL(18,2) = CAST(((@i + @m * 5) % 15) + 1 AS DECIMAL(18,2))

        INSERT INTO [PurchaseReturnItem] ([PurchaseReturnID], [BatchID], [Quantity])
        VALUES (@pr_id, @batch_pr, @qty_pr)

        -- Giam ton kho
        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] - @qty_pr
        WHERE [BatchID] = @batch_pr AND [QuantityInStock] >= @qty_pr

        SET @m = @m + 1
    END
    SET @i = @i + 1
END

-- =============================================================================
-- 19. SALES RETURNS (50 phieu khach hang tra hang)
-- =============================================================================
PRINT '19/27 - Dang tao SalesReturn...'
SET @i = 1
WHILE @i <= 50
BEGIN
    DECLARE @sr_id BIGINT
    INSERT INTO [SalesReturn] ([CustomerID], [UserID], [CreatedAt], [Note])
    VALUES (
        ((@i - 1) % 400) + 1,
        7 + ((@i - 1) % 4),
        DATEADD(DAY, ((@i - 1) % 365), '2024-06-01'),
        N'Phieu khach tra hang so ' + CAST(@i AS NVARCHAR(10))
    )
    SET @sr_id = SCOPE_IDENTITY()

    -- 1-3 chi tiet tra
    DECLARE @sr_item_count INT = (@i % 3) + 1
    DECLARE @n INT = 1
    WHILE @n <= @sr_item_count
    BEGIN
        DECLARE @batch_sr BIGINT = ((@i * 5 + @n * 11) % 1200) + 1
        DECLARE @qty_sr DECIMAL(18,2) = CAST(((@i + @n * 3) % 10) + 1 AS DECIMAL(18,2))

        INSERT INTO [SalesReturnItem] ([SalesReturnID], [BatchID], [Quantity])
        VALUES (@sr_id, @batch_sr, @qty_sr)

        -- Tang ton kho (khach tra lai)
        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] + @qty_sr
        WHERE [BatchID] = @batch_sr

        SET @n = @n + 1
    END
    SET @i = @i + 1
END

-- =============================================================================
-- 20. STOCK TAKE (50 phieu kiem ke)
-- =============================================================================
PRINT '20/27 - Dang tao StockTake...'
SET @i = 1
WHILE @i <= 50
BEGIN
    DECLARE @st_id BIGINT
    DECLARE @wh_st INT = ((@i - 1) % 4) + 1
    INSERT INTO [StockTake] ([WarehouseID], [UserID], [CreatedAt], [Note], [IsBalance])
    VALUES (
        @wh_st,
        CASE @wh_st WHEN 1 THEN 11 WHEN 2 THEN 12 WHEN 3 THEN 13 WHEN 4 THEN 14 END,
        DATEADD(DAY, ((@i - 1) % 365), '2024-06-01'),
        N'Kiem ke ky so ' + CAST(@i AS NVARCHAR(10)),
        CASE (@i % 5) WHEN 0 THEN 1 ELSE 0 END  -- 0=Balanced, 1=Difference
    )
    SET @st_id = SCOPE_IDENTITY()

    -- 2-5 chi tiet kiem ke
    DECLARE @st_item_count INT = (@i % 4) + 2
    DECLARE @p INT = 1
    WHILE @p <= @st_item_count
    BEGIN
        DECLARE @batch_st BIGINT = ((@i * 7 + @p * 13) % 1200) + 1
        DECLARE @sys_qty DECIMAL(18,2)
        DECLARE @act_qty DECIMAL(18,2)
        DECLARE @diff_qty DECIMAL(18,2)

        SELECT @sys_qty = [QuantityInStock] FROM [Batch] WHERE [BatchID] = @batch_st
        IF @sys_qty IS NULL SET @sys_qty = 100

        SET @act_qty = CASE (@i % 5)
            WHEN 0 THEN @sys_qty  -- can bang
            ELSE @sys_qty + CASE (@p % 2) WHEN 0 THEN 5 ELSE -5 END  -- lech
        END
        SET @diff_qty = @act_qty - @sys_qty

        INSERT INTO [StockTakeItem] ([StockTakeID], [BatchID], [SystemQuantity], [ActualQuantity], [DifferenceQuantity])
        VALUES (@st_id, @batch_st, @sys_qty, @act_qty, @diff_qty)

        SET @p = @p + 1
    END
    SET @i = @i + 1
END

-- =============================================================================
-- 21. STOCK ADJUSTMENT (50 phieu dieu chinh)
-- =============================================================================
PRINT '21/27 - Dang tao StockAdjustment...'
SET @i = 1
WHILE @i <= 50
BEGIN
    DECLARE @sa_id BIGINT
    DECLARE @wh_sa INT = ((@i - 1) % 4) + 1
    DECLARE @st_ref BIGINT = @i  -- 1-to-1 voi StockTake

    INSERT INTO [StockAdjustment] ([WarehouseID], [UserID], [StockTakeID], [Note], [CreatedAt])
    VALUES (
        @wh_sa,
        CASE @wh_sa WHEN 1 THEN 11 WHEN 2 THEN 12 WHEN 3 THEN 13 WHEN 4 THEN 14 END,
        @st_ref,
        N'Dieu chinh sau kiem ke so ' + CAST(@i AS NVARCHAR(10)),
        DATEADD(DAY, 1, DATEADD(DAY, ((@i - 1) % 365), '2024-06-01'))
    )
    SET @sa_id = SCOPE_IDENTITY()

    -- 2-3 chi tiet dieu chinh
    DECLARE @sa_item_count INT = (@i % 2) + 2
    DECLARE @q INT = 1
    WHILE @q <= @sa_item_count
    BEGIN
        DECLARE @batch_sa BIGINT = ((@i * 11 + @q * 7) % 1200) + 1
        DECLARE @adj_qty DECIMAL(18,2) = CASE (@q % 2) WHEN 0 THEN 5.0 ELSE -5.0 END

        INSERT INTO [StockAdjustmentItem] ([StockAdjustmentID], [BatchID], [AdjustQuantity])
        VALUES (@sa_id, @batch_sa, @adj_qty)

        -- Cap nhat ton kho
        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] + @adj_qty
        WHERE [BatchID] = @batch_sa AND [QuantityInStock] + @adj_qty >= 0

        SET @q = @q + 1
    END
    SET @i = @i + 1
END

-- =============================================================================
-- 22. DESTROY RECEIPT (50 phieu huy thuoc)
-- =============================================================================
PRINT '22/27 - Dang tao DestroyReceipt...'
SET @i = 1
WHILE @i <= 50
BEGIN
    DECLARE @dr_id BIGINT
    DECLARE @wh_dr INT = ((@i - 1) % 4) + 1
    DECLARE @st_ref_dr BIGINT = @i  -- 1-to-1 voi StockTake

    INSERT INTO [DestroyReceipt] ([WarehouseID], [UserID], [StockTakeID], [Note], [CreatedAt])
    VALUES (
        @wh_dr,
        CASE @wh_dr WHEN 1 THEN 11 WHEN 2 THEN 12 WHEN 3 THEN 13 WHEN 4 THEN 14 END,
        @st_ref_dr,
        N'Phieu huy thuoc so ' + CAST(@i AS NVARCHAR(10)),
        DATEADD(DAY, ((@i - 1) % 365), '2024-06-01')
    )
    SET @dr_id = SCOPE_IDENTITY()

    -- 1-2 chi tiet huy
    DECLARE @dr_item_count INT = (@i % 2) + 1
    DECLARE @r INT = 1
    WHILE @r <= @dr_item_count
    BEGIN
        DECLARE @batch_dr BIGINT = ((@i * 9 + @r * 5) % 1200) + 1
        DECLARE @qty_dr DECIMAL(18,2) = CAST(((@i + @r * 2) % 10) + 1 AS DECIMAL(18,2))

        INSERT INTO [DestroyReceiptItem] ([DestroyReceiptID], [BatchID], [Quantity])
        VALUES (@dr_id, @batch_dr, @qty_dr)

        -- Giam ton kho
        UPDATE [Batch]
        SET [QuantityInStock] = [QuantityInStock] - @qty_dr
        WHERE [BatchID] = @batch_dr AND [QuantityInStock] >= @qty_dr

        SET @r = @r + 1
    END
    SET @i = @i + 1
END

-- =============================================================================
-- 23. DAM BAO TON KHONG AM (Fix any negative stock)
-- =============================================================================
PRINT '23/27 - Dang kiem tra ton kho...'
UPDATE [Batch]
SET [QuantityInStock] = 0
WHERE [QuantityInStock] < 0

-- =============================================================================
-- 24. XAC NHAN DU LIEU
-- =============================================================================


-- =============================================================================
-- 25. HOAN THANH
-- =============================================================================
PRINT '25/27 - HOAN THANH! Du lieu mau da duoc tao thanh cong.'

COMMIT TRANSACTION
GO

select * from [User]

-- lấy role theo username
select Permission.Name, u.UserName
from [User] as u
JOIN UserBranch on u.UserID = UserBranch.UserID
JOIN Role on UserBranch.RoleID = Role.RoleID
JOIN RolePermission on RolePermission.RoleID = Role.RoleID
JOIN Permission on Permission.PermissionID = RolePermission.PermissionID
where u.UserName = 'user0001'

select * from Branch