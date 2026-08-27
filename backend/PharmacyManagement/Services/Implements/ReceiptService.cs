using Microsoft.AspNetCore.Http;
using PharmacyManagement.DTOs.Receipt;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;

namespace PharmacyManagement.Services.Implements
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;

        public ReceiptService(IReceiptRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request, long userId)
        {
            if (request.TotalAmount <= 0)
                throw new BusinessException("TotalAmount must be greater than zero.", "RCP001", StatusCodes.Status400BadRequest);

            if (!await _repository.IsCustomerExistsAsync(request.CustomerID))
                throw new BusinessException("Customer not found.", "RCP002", StatusCodes.Status404NotFound);

            var entity = request.ToEntity(userId);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var saved = await _repository.GetByIdAsync(entity.ReceiptID);
            return saved!.ToResponse();
        }

        public async Task<ReceiptResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<ReceiptListResponse> GetAllAsync(long? customerId, int page, int count)
        {
            var skip = PaginationHelper.GetSkip(page, count);

            var items = await _repository.GetAllAsync(skip, count, customerId);
            var numRecords = await _repository.CountAsync(customerId);

            return new ReceiptListResponse
            {
                NumRecords = numRecords,
                TotalPage = (int)PaginationHelper.GetTotalPage(numRecords, count),
                Items = items.ToResponseList()
            };
        }
    }
}
