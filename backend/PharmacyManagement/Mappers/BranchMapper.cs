using PharmacyManagement.DTOs.Branch;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class BranchMapper
    {
        public static Branch ToEntity(this CreateBranchRequest request)
        {
            return new Branch
            {
                BranchName = request.BranchName,
                Phone = request.Phone,
                Address = request.Address,
                PriceListID = request.PriceListID,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static void ApplyTo(this UpdateBranchRequest request, Branch entity)
        {
            entity.BranchName = request.BranchName;
            entity.Phone = request.Phone;
            entity.Address = request.Address;
            entity.IsActive = request.IsActive;
            if (request.PriceListID.HasValue)
                entity.PriceListID = request.PriceListID.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static BranchResponse ToResponse(this Branch entity)
        {
            return new BranchResponse
            {
                BranchID = entity.BranchID,
                BranchName = entity.BranchName,
                Phone = entity.Phone,
                Address = entity.Address,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsActive = entity.IsActive,
                PriceListID = entity.PriceListID
            };
        }

        public static List<BranchResponse> ToResponseList(this List<Branch> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
