using PharmacyManagement.DTOs.Receipt;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class ReceiptMapper
    {
        public static Receipt ToEntity(this CreateReceiptRequest request, long userId)
        {
            return new Receipt
            {
                CustomerID = request.CustomerID,
                UserID = userId,
                TotalAmount = request.TotalAmount,
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
            return new ReceiptResponse
            {
                ReceiptID = entity.ReceiptID,
                CustomerID = entity.CustomerID,
                CustomerName = entity.Customer?.CustomerName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                TotalAmount = entity.TotalAmount,
                CreatedDate = entity.CreatedDate
            };
        }

        public static List<ReceiptResponse> ToResponseList(this IEnumerable<Receipt> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
