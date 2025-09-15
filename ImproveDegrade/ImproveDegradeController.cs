using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.ImproveDegrade
{
    [ApiController]
    [Route("v1")]
    public class ImproveDegradeController : ControllerBase
    {
        private readonly IImproveDegradeService _service;

        public ImproveDegradeController(IImproveDegradeService service)
        {
            _service = service;
        }

        [HttpGet("improve-degrade")]
        public async Task<ActionResult<BaseResponse>> GetWinLoseMetrics(
            [FromQuery] ImproveDegradeRequest request
        )
        {
            var result = await _service.GetImproveDegrade(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }

        [HttpGet("improve-degrade/excel")]
        public async Task<IActionResult> DownloadWinLoseExcel([FromQuery] ImproveDegradeRequest request)
        {
            var excelData = await _service.GenerateWinLoseExcelFile(request);
            var fileName = $"ImproveDegrade_{request.Level}_{request.Location}_{request.Yearweek}.xlsx";
            
            return File(excelData, 
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                       fileName);
        }
    }
}
