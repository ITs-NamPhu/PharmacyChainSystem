using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.GoodsReceipt;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceiptController : BaseController
    {
        private readonly IGoodsReceiptService _service;

        public GoodsReceiptController(IGoodsReceiptService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("GOODSRECEIPT_CREATE")]
        public async Task<IActionResult> Create(CreateGoodsReceiptRequest request)
        {
            var branchId = GetBranchIdFromHeader();
            var userId = GetUserIdFromToken();
            var result = await _service.CreateAsync(request, userId, branchId);
            return Success(result, "Create goods receipt successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("GOODSRECEIPT_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateGoodsReceiptRequest request)
        {
            var branchId = GetBranchIdFromHeader();
            var userId = GetUserIdFromToken();
            var result = await _service.UpdateAsync(id, request, userId, branchId);
            return Success(result, "Update goods receipt successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("GOODSRECEIPT_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            var branchId = GetBranchIdFromHeader();
            await _service.DeleteAsync(id, branchId);
            return Success(null, "Delete goods receipt successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("GOODSRECEIPT_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var branchId = GetBranchIdFromHeader();
            var result = await _service.GetByIdAsync(id, branchId);
            return Success(result, "Get goods receipt successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("GOODSRECEIPT_VIEW")]
        public async Task<IActionResult> GetAll([FromQuery] GoodsReceiptFilterDto filter)
        {
            var branchId = GetBranchIdFromHeader();
            var result = await _service.GetAllAsync(filter ?? new GoodsReceiptFilterDto(), branchId);
            return Success(result, "Get goods receipts successfully.");
        }
    }
}
