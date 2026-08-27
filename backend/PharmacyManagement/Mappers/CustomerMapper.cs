using PharmacyManagement.DTOs.Customer;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class CustomerMapper
    {
        public static Customer ToEntity(this CreateCustomerRequest request)
        {
            return new Customer
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Address = request.Address,
                Email = request.Email,
                CustomerTypeID = request.CustomerTypeID
            };
        }

        public static void ApplyTo(this UpdateCustomerRequest request, Customer entity)
        {
            entity.CustomerName = request.CustomerName;
            entity.Phone = request.Phone;
            entity.Address = request.Address;
            entity.Email = request.Email;
            entity.CustomerTypeID = request.CustomerTypeID;
        }

        public static CustomerResponse ToResponse(this Customer entity)
        {
            return new CustomerResponse
            {
                CustomerID = entity.CustomerID,
                CustomerName = entity.CustomerName,
                Phone = entity.Phone ?? "",
                Address = entity.Address ?? "",
                Email = entity.Email ?? "",
                CustomerTypeID = entity.CustomerTypeID,
                CustomerTypeName = entity.CustomerType?.TypeName
            };
        }

        public static List<CustomerResponse> ToResponseList(this List<Customer> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
