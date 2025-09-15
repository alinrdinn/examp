using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.Summary.Dtos.Requests;
using ExecutiveDashboard.Modules.Summary.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.Summary
{
    [ApiController]
    [Route("v1")]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _service;

        public SummaryController(ISummaryService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<BaseResponse>> GetSummary([FromQuery] SummaryRequest request)
        {
            var result = await _service.GetSummary(request);

            var response = BaseResponse.ToResponse(200, true, result, null);
            return Ok(response);
        }

        [HttpGet("summary/excel")]
        public async Task<IActionResult> DownloadSummary([FromQuery] SummaryRequest request)
        {
            var fileBytes = await _service.GenerateSummaryExcelFile(request);

            var fileName = $"Summary_{request.Level}_{request.Location}_{request.Yearweek}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
