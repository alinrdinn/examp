using ClosedXML.Excel;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses;

namespace ExecutiveDashboard.Modules.OLOPerformance.Services
{
    public interface IOLOPerformanceService
    {
        Task<OLOPerformanceResponse> GetOloPerformance(OLOPerformanceRequest request);
        Task<OLOPerformanceSummaryResponse> GetOloPerformanceSummary(OLOPerformanceRequest request);
        Task<IXLWorksheet> CreateOloPerformanceWorksheet(
            XLWorkbook workbook,
            OLOPerformanceRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateOloPerformanceWorkbook(OLOPerformanceRequest request);
        Task<byte[]> GenerateOloPerformanceExcelFile(OLOPerformanceRequest request);

        Task<IXLWorksheet> CreateOloPerformanceSummaryWorksheet(
            XLWorkbook workbook,
            OLOPerformanceRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateOloPerformanceSummaryWorkbook(OLOPerformanceRequest request);
        Task<byte[]> GenerateOloPerformanceSummaryExcelFile(OLOPerformanceRequest request);
    }
}
