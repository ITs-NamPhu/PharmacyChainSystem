using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.DestroyReceipt;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestroyReceiptController : BaseController
    {
        private readonly IDestroyReceiptService _service;

        public DestroyReceiptController(IDestroyReceiptService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("DESTROY_CREATE")]
        public async Task<IActionResult> Create(CreateDestroyReceiptRequest request)
        {
            var userId = GetUserIdFromToken();
            var result = await _service.CreateAsync(request, userId);
            return Success(result, "Create destroy receipt successfully.");
        }

        [HttpPost("{id}/approve")]
        [HasPermission("DESTROY_APPROVE")]
        public async Task<IActionResult> Approve(long id)
        {
            var userId = GetUserIdFromToken();
            await _service.ApproveAsync(id, userId);
            return Success(null, "Approve destroy receipt successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("DESTROY_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get destroy receipt successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("DESTROY_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10, long warehouseId = 0)
        {
            var result = await _service.GetAllAsync(page, count, warehouseId);
            return Success(result, "Get destroy receipts successfully.");
        }
    }
}
