using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.PermissionHandle;

namespace PharmacyManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDebtSummaryController : BaseController
    {
        private readonly IDebtSummaryService _service;

        public CustomerDebtSummaryController(IDebtSummaryService service)
        {
            _service = service;
        }

        [HttpPost("close")]
        [HasPermission("CUSTOMER_DEBT_CLOSE")]
        public async Task<IActionResult> Close(int year, int month)
        {
            await _service.ProcessMonthlyClosingAsync(year, month);
            return Success(null, "Debt summary closed successfully.");
        }

        [HttpGet]
        [HasPermission("CUSTOMER_DEBT_VIEW")]
        public async Task<IActionResult> GetByMonth(int year, int month, long? customerId, int page = 1, int count = 10)
        {
            var result = await _service.GetByMonthAsync(year, month, customerId, page, count);
            return Success(result, "Get debt summaries successfully.");
        }
    }
}
