using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests;
using ExecutiveDashboard.Modules.MostLessWin.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.MostLessWin
{
    [ApiController]
    [Route("v1")]
    public class MostLessWinController : ControllerBase
    {
        private readonly IMostLessWinService _service;

        public MostLessWinController(IMostLessWinService service)
        {
            _service = service;
        }

        [HttpGet("most-less-win")]
        public async Task<ActionResult<BaseResponse>> GetMostLessWin(
            [FromQuery] MostLessWinRequest request
        )
        {
            var result = await _service.GetMostLessWin(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }

        [HttpGet("most-less-win/excel")]
        public async Task<IActionResult> DownloadMostLessWinExcel(
            [FromQuery] MostLessWinRequest request
        )
        {
            var excelData = await _service.GenerateWinLoseMetricsExcelFile(request);
            
            var fileName = $"win-lose-metrics-{request.Yearweek}.xlsx";
            
            return File(excelData, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
    }
}
