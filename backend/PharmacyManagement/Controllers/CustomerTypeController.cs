using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.CustomerType;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTypeController : BaseController
    {
        private readonly ICustomerTypeService _service;

        public CustomerTypeController(ICustomerTypeService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("CUSTOMERTYPE_CREATE")]
        public async Task<IActionResult> Create(CreateCustomerTypeRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create customer type successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("CUSTOMERTYPE_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateCustomerTypeRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update customer type successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("CUSTOMERTYPE_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete customer type successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("CUSTOMERTYPE_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get customer type successfully.");
        }

        [HttpGet("All")]
        [HasPermission("CUSTOMERTYPE_VIEW")]
        public async Task<IActionResult> GetAllCustomerType()
        {
            var result = await _service.GetAllCustomerTypeAsync();
            return Success(result, "Get all customer types successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("CUSTOMERTYPE_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get customer types successfully.");
        }
    }
}
