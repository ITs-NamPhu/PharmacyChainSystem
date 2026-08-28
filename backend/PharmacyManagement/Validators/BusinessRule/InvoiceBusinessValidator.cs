using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class InvoiceBusinessValidator
    {
        private readonly IInvoiceRepository _repository;
        private readonly ICustomerRepository _customerRepository;

        public InvoiceBusinessValidator(IInvoiceRepository repository, ICustomerRepository customerRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
        }

        public async Task<Models.Customer> ValidateCustomerExistsAsync(long customerId)
        {
            Models.Customer customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new BusinessException(
                    "Customer not found.",
                    "INV001",
                    StatusCodes.Status404NotFound);
            }
            return customer;
        }

        public async Task ValidateBatchExistsAsync(long batchId)
        {
            var batch = await _repository.GetBatchByIdAsync(batchId);
            if (batch == null)
            {
                throw new BusinessException(
                    "Batch not found.",
                    "INV002",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateBatchBelongsToMedicineAsync(long batchId, long medicineId)
        {
            var batch = await _repository.GetBatchByIdAsync(batchId);
            if (batch == null)
            {
                throw new BusinessException(
                    "Batch not found.",
                    "INV002",
                    StatusCodes.Status404NotFound);
            }

            if (batch.GoodsReceiptItem?.MedicineID != medicineId)
            {
                throw new BusinessException(
                    "Batch does not belong to the selected medicine.",
                    "INV003",
                    StatusCodes.Status400BadRequest);
            }
        }

        public async Task ValidateQuantityInStockAsync(long batchId, decimal quantity)
        {
            var batch = await _repository.GetBatchByIdAsync(batchId);
            if (batch == null)
            {
                throw new BusinessException(
                    "Batch not found.",
                    "INV002",
                    StatusCodes.Status404NotFound);
            }

            if (batch.QuantityInStock < quantity)
            {
                throw new BusinessException(
                    $"Insufficient stock. Available: {batch.QuantityInStock}, Requested: {quantity}.",
                    "INV004",
                    StatusCodes.Status400BadRequest);
            }
        }

        public async Task ValidateFefoQuantityAvailableAsync(long medicineId, decimal quantity, long branchId)
        {
            var batches = await _repository.GetBatchesByMedicineAsync(medicineId, branchId);
            var totalStock = batches.Sum(b => b.QuantityInStock);

            if (totalStock < quantity)
            {
                throw new BusinessException(
                    $"Insufficient stock for medicine (ID: {medicineId}). Available: {totalStock}, Requested: {quantity}.",
                    "INV005",
                    StatusCodes.Status400BadRequest);
            }
        }
    }
}
