using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.OLOPerformance.Services
{
    public interface IOLOPerformanceService
    {
        Task<OLOPerformanceResponse> GetOloPerformance(OLOPerformanceRequest request);
        Task<XLWorkbook> GenerateOloPerformanceWorkbook(OLOPerformanceRequest request);
        Task<byte[]> GenerateOloPerformanceExcelFile(OLOPerformanceRequest request);
    }
}
