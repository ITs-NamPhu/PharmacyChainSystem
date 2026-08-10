using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.UnitConversion;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitConversionController : BaseController
    {
        private readonly IUnitConversionService _service;

        public UnitConversionController(IUnitConversionService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("UnitConversion_Create")]
        public async Task<IActionResult> Create(CreateUnitConversionRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create unit conversion successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("UnitConversion_Update")]
        public async Task<IActionResult> Update(long id, UpdateUnitConversionRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update unit conversion successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("UnitConversion_Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete unit conversion successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("UnitConversion_View")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get unit conversion successfully.");
        }

        [HttpGet("All")]
        [HasPermission("UnitConversion_View")]
        public async Task<IActionResult> GetAllUnitConversion()
        {
            var result = await _service.GetAllUnitConversionAsync();
            return Success(result, "Get all unit conversions successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("UnitConversion_View")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get unit conversions successfully.");
        }

        [HttpGet("GetList")]
        [HasPermission("UnitConversion_View")]
        // Lấy danh sách unit conversion và medicine name -> hiển thị trong dropdown khi tạo invoice
        public async Task<IActionResult> GetListUnitConversion_Medicine()
        {
            var result = await _service.GetListUnitConversion_MedicineAsync();
            return Success(result, "Get unit conversions successfully.");
        }
    }
}
