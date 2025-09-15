using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Requests;
using ExecutiveDashboard.Modules.WinLoseMetric.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.WinLoseMetric
{
    [ApiController]
    [Route("v1")]
    public class WinLoseMetricController : ControllerBase
    {
        private readonly IWinLoseMetricService _service;

        public WinLoseMetricController(IWinLoseMetricService service)
        {
            _service = service;
        }

        [HttpGet("win-lose-metrics")]
        public async Task<ActionResult<BaseResponse>> GetWinLoseMetrics(
            [FromQuery] WinLoseMetricRequest request
        )
        {
            var result = await _service.GetWinLoseMetrics(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }
        
        [HttpGet("win-lose-metrics/excel")]
        public async Task<ActionResult> DownloadWinLoseMetricsExcel(
            [FromQuery] WinLoseMetricRequest request
        )
        {
            var excelBytes = await _service.GenerateWinLoseMetricsExcelFile(request);
                
                var fileName = $"winlose-metrics-{request.Yearweek}-{request.Location}.xlsx";
                
                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
        }
    }
}
