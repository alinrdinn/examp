using ClosedXML.Excel;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses;

namespace ExecutiveDashboard.Modules.OLOPerformance.Services
{
    public interface IOLOPerformanceService
    {
        Task<OLOPerformanceResponse> GetOloPerformance(OLOPerformanceRequest request);
        Task<OLOPerformanceSummaryResponse> GetOloPerformanceSummary(OLOPerformanceRequest request);
        Task<XLWorkbook> GenerateOloPerformanceWorkbook(OLOPerformanceRequest request);
        Task<byte[]> GenerateOloPerformanceExcelFile(OLOPerformanceRequest request);

        Task<XLWorkbook> GenerateOloPerformanceSummaryWorkbook(OLOPerformanceRequest request);
        Task<byte[]> GenerateOloPerformanceSummaryExcelFile(OLOPerformanceRequest request);
    }
}
