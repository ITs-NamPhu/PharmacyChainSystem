using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.Manufacturer;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturerController : BaseController
    {
        private readonly IManufacturerService _service;

        public ManufacturerController(IManufacturerService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("MANUFACTURER_CREATE")]
        public async Task<IActionResult> Create(CreateManufacturerRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create manufacturer successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("MANUFACTURER_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateManufacturerRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update manufacturer successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("MANUFACTURER_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete manufacturer successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("MANUFACTURER_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get manufacturer successfully.");
        }

        [HttpGet("All")]
        [HasPermission("MANUFACTURER_VIEW")]
        public async Task<IActionResult> GetAllManufacturer()
        {
            var result = await _service.GetAllManufacturerAsync();
            return Success(result, "Get all manufacturers successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("MANUFACTURER_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get manufacturers successfully.");
        }
    }
}
