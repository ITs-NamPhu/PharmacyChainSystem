using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.Supplier;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("SUPPLIER_CREATE")]
        public async Task<IActionResult> Create(CreateSupplierRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create supplier successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("SUPPLIER_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateSupplierRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update supplier successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("SUPPLIER_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete supplier successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("SUPPLIER_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get supplier successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("SUPPLIER_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get suppliers successfully.");
        }
    }
}
