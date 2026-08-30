using PharmacyManagement.DTOs.Receipt;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class ReceiptMapper
    {
        public static Receipt ToEntity(this CreateReceiptRequest request, long userId, long branchId)
        {
            return new Receipt
            {
                CustomerID = request.CustomerID,
                BranchID = branchId,
                UserID = userId,
                TotalAmount = request.TotalAmount,
                PaymentMethod = PaymentMethod.CASH,
                CreatedDate = DateTime.Now
            };
        }

        public static void ApplyTo(this UpdateReceiptRequest request, Receipt entity)
        {
            entity.CustomerID = request.CustomerID;
            entity.TotalAmount = request.TotalAmount;
        }

        public static ReceiptResponse ToResponse(this Receipt entity)
        {
            var details = entity.ReceiptDetail?.ToList() ?? new List<ReceiptDetail>();
            var appliedTotal = details.Sum(d => d.AmountApplied);

            return new ReceiptResponse
            {
                ReceiptID = entity.ReceiptID,
                CustomerID = entity.CustomerID,
                CustomerName = entity.Customer?.CustomerName ?? string.Empty,
                BranchID = entity.BranchID,
                BranchName = entity.Branch?.BranchName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                TotalAmount = entity.TotalAmount,
                PaymentMethod = entity.PaymentMethod,
                AmountApplied = appliedTotal,
                WalletCredit = Math.Max(0, entity.TotalAmount - appliedTotal),
                RemainingDebt = 0,
                CreatedDate = entity.CreatedDate,
                Details = details.ToResponseList()
            };
        }

        public static List<ReceiptResponse> ToResponseList(this IEnumerable<Receipt> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public static ReceiptDetailResponse ToResponse(this ReceiptDetail detail)
        {
            return new ReceiptDetailResponse
            {
                ReceiptDetailID = detail.ReceiptDetailID,
                InvoiceID = detail.InvoiceID,
                AmountApplied = detail.AmountApplied,
                InvoiceCreatedAt = detail.Invoice?.CreatedAt ?? DateTime.MinValue,
                InvoiceTotalAmount = detail.Invoice?.TotalAmount ?? 0,
                InvoicePaymentStatus = detail.Invoice?.PaymentStatus ?? PaymentStatus.Debt
            };
        }

        public static List<ReceiptDetailResponse> ToResponseList(this IEnumerable<ReceiptDetail> details)
        {
            return details.Select(d => d.ToResponse()).ToList();
        }
    }
}