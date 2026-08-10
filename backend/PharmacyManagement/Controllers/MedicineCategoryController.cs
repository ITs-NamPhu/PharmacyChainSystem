using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.MedicineCategory;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineCategoryController : BaseController
    {
        private readonly IMedicineCategoryService _service;

        public MedicineCategoryController(IMedicineCategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("MEDICINECATEGORY_CREATE")]
        public async Task<IActionResult> Create(CreateMedicineCategoryRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create medicine category successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("MEDICINECATEGORY_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateMedicineCategoryRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update medicine category successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("MEDICINECATEGORY_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete medicine category successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("MEDICINECATEGORY_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get medicine category successfully.");
        }

        [HttpGet("All")]
        [HasPermission("MEDICINECATEGORY_VIEW")]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _service.GetAllCategoryAsync();
            return Success(result, "Get all medicine categories successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("MEDICINECATEGORY_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get medicine categories successfully.");
        }
    }
}
