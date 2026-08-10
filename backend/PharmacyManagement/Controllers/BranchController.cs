using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.Branch;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : BaseController
    {
        private readonly IBranchService _service;

        public BranchController(IBranchService service)
        {
            _service = service;
        }

        [HttpPost]
        [HasPermission("BRANCH_CREATE")]
        public async Task<IActionResult> Create(CreateBranchRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Success(result, "Create branch successfully.");
        }

        [HttpPut("{id}")]
        [HasPermission("BRANCH_UPDATE")]
        public async Task<IActionResult> Update(long id, UpdateBranchRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return Success(result, "Update branch successfully.");
        }

        [HttpDelete("{id}")]
        [HasPermission("BRANCH_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return Success(null, "Delete branch successfully.");
        }

        [HttpGet("{id}")]
        [HasPermission("BRANCH_VIEW")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Success(result, "Get branch successfully.");
        }

        [HttpGet("All")]
        [HasPermission("BRANCH_VIEW")]
        public async Task<IActionResult> GetAllBranch()
        {
            var result = await _service.GetAllBranchAsync();
            return Success(result, "Get all branches successfully.");
        }

        [HttpGet("GetAll")]
        [HasPermission("BRANCH_VIEW")]
        public async Task<IActionResult> GetAll(int page, int count = 10)
        {
            var result = await _service.GetAllAsync(page, count);
            return Success(result, "Get branches successfully.");
        }
    }
}
