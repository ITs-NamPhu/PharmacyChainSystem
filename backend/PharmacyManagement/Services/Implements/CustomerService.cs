using PharmacyManagement.DTOs.Customer;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerBusinessValidator _businessValidator;

        public CustomerService(ICustomerRepository repository, CustomerBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            await _businessValidator.ValidateCustomerTypeExistsAsync(request.CustomerTypeID);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<CustomerResponse> UpdateAsync(long id, UpdateCustomerRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer not found.", "CUST004", StatusCodes.Status404NotFound);

            await _businessValidator.ValidateCustomerTypeExistsAsync(request.CustomerTypeID);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer not found.", "CUST004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<CustomerResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<CustomerListResponse> GetAllAsync(int page, int count)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                _repository.GetAllAsync, _repository.CountAsync, page, count);

            return new CustomerListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Customers = paged.Items.ToResponseList()
            };
        }

        public async Task<List<CustomerResponse>> GetAllCustomerAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<CustomerWalletResponse> GetWalletAsync(long customerId)
        {
            var customer = await _repository.GetByIdAsync(customerId);
            if (customer == null)
                throw new BusinessException("Customer not found.", "CUST004", StatusCodes.Status404NotFound);

            // Lấy lịch sử gần nhất (20 dòng gần đây nhất của sổ phụ ví)
            var history = await _repository.GetWalletHistoryAsync(customerId, 0, 20);

            return new CustomerWalletResponse
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName,
                WalletBalance = customer.WalletBalance,
                History = history.Select(h => new CustomerWalletHistoryResponse
                {
                    CustomerWalletHistoryID = h.CustomerWalletHistoryID,
                    TransactionType = h.TransactionType,
                    Amount = h.Amount,
                    RefType = h.RefType,
                    RefId = h.RefId,
                    CreateDate = h.CreateDate
                }).ToList()
            };
        }
    }
}
