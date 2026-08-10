using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class Batch
    {
        public long BatchID { get; set; }
        public long GoodsReceiptItemID { get; set; }
        public long WarehouseID { get; set; }

        // số lượng nhập
        public decimal QuantityReceived { get; set; }

        // số lượng còn lại
        public decimal QuantityInStock { get; set; }

        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [StringLength(255)]
        public String Note { get; set; }

        public GoodsReceiptItem? GoodsReceiptItem { get; set; }
        public WareHouse? WareHouse { get; set; }
        public ICollection<InvoiceItem>? InvoiceItem { get; set; }
        public ICollection<PurchaseReturnItem>? PurchaseReturnItem { get; set; }
        public ICollection<SalesReturnItem>? SalesReturnItem { get; set; }
        public ICollection<DestroyReceiptItem>? DestroyReceiptItem { get; set; }
        public ICollection<StockAdjustmentItem>? StockAdjustmentItem { get; set; }
        public ICollection<StockTakeItem>? StockTakeItem { get; set; }
        public ICollection<InventoryTransaction>? InventoryTransaction { get; set; }

    }
}