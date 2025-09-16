using ClosedXML.Excel;
using ExecutiveDashboard.Modules.ExcelExport.Dtos.Requests;

namespace ExecutiveDashboard.Modules.ExcelExport.Services
{
    public interface IExcelExportService
    {
        Task<XLWorkbook> GenerateExcelExportWorkbook(ExcelExportRequest request);
        Task<byte[]> GenerateExcelExportFile(ExcelExportRequest request);
    }
}
