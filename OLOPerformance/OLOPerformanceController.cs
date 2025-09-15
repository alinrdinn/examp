using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.OLOPerformance
{
    [ApiController]
    [Route("v1")]
    public class OLOPerformanceController : ControllerBase
    {
        private readonly IOLOPerformanceService _service;

        public OLOPerformanceController(IOLOPerformanceService service)
        {
            _service = service;
        }

        [HttpGet("olo-performance")]
        public async Task<ActionResult<BaseResponse>> GetOLOPerformance(
            [FromQuery] OLOPerformanceRequest request
        )
        {
            var result = await _service.GetOloPerformance(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }

        [HttpGet("olo-performance/excel")]
        public async Task<IActionResult> GetOLOPerformanceExcel(
            [FromQuery] OLOPerformanceRequest request
        )
        {
            var excelBytes = await _service.GenerateOloPerformanceExcelFile(request);

            var fileName = $"olo-performance-{request.Yearweek}-{request.Level}-{request.Location}.xlsx";
            
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
