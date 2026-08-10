using PharmacyManagement.DTOs.Supplier;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class SupplierMapper
    {
        public static Supplier ToEntity(this CreateSupplierRequest request)
        {
            return new Supplier
            {
                SupplierName = request.SupplierName,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address
            };
        }

        public static void ApplyTo(this UpdateSupplierRequest request, Supplier entity)
        {
            entity.SupplierName = request.SupplierName;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.Address = request.Address;
        }

        public static SupplierResponse ToResponse(this Supplier entity)
        {
            return new SupplierResponse
            {
                SupplierID = entity.SupplierID,
                SupplierName = entity.SupplierName,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address
            };
        }

        public static List<SupplierResponse> ToResponseList(this List<Supplier> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
