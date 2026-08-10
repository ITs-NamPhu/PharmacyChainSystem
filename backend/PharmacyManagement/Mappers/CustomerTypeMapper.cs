using PharmacyManagement.DTOs.CustomerType;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class CustomerTypeMapper
    {
        public static CustomerType ToEntity(this CreateCustomerTypeRequest request)
        {
            return new CustomerType
            {
                TypeName = request.TypeName,
                DiscountPercent = request.DiscountPercent
            };
        }

        public static void ApplyTo(this UpdateCustomerTypeRequest request, CustomerType entity)
        {
            entity.TypeName = request.TypeName;
            entity.DiscountPercent = request.DiscountPercent;
        }

        public static CustomerTypeResponse ToResponse(this CustomerType entity)
        {
            return new CustomerTypeResponse
            {
                CustomerTypeID = entity.CustomerTypeID,
                TypeName = entity.TypeName,
                DiscountPercent = entity.DiscountPercent
            };
        }

        public static List<CustomerTypeResponse> ToResponseList(this List<CustomerType> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
