using ExecutiveDashboard.Modules.ExcelExport.Dtos.Requests;
using ExecutiveDashboard.Modules.ExcelExport.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Modules.ExcelExport
{
    [ApiController]
    [Route("v1")]
    public class ExcelExportController : ControllerBase
    {
        private readonly IExcelExportService _service;

        public ExcelExportController(IExcelExportService service)
        {
            _service = service;
        }

        [HttpGet("excel-export")]
        public async Task<IActionResult> DownloadExcel([FromQuery] ExcelExportRequest request)
        {
            var excelBytes = await _service.GenerateExcelExportFile(request);

            var fileName = $"dashboard-export-{request.Yearweek}-{request.Location}.xlsx";

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
