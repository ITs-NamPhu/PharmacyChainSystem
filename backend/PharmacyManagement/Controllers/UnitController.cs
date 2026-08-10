using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.Unit;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : BaseController
    {
        private readonly IUnitService _service;

        public UnitController(IUnitService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("UNIT_CREATE")]
        public async Task<IActionResult> Create(CreateUnitRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create unit successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("UNIT_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateUnitRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update unit successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("UNIT_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete unit successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("UNIT_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get unit successfully.");
        }

        [HttpGet("All")]
        [HasPermission("UNIT_VIEW")]
        public async Task<IActionResult> GetAllUnit()
        {
            var result = await _service.GetAllUnitAsync();
            return Success(result, "Get all units successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("UNIT_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get units successfully.");
        }
    }
}
