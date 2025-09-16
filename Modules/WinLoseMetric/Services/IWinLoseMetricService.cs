using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Requests;
using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Responses;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Services
{
    public interface IWinLoseMetricService
    {
        Task<WinLoseMetricResponse> GetWinLoseMetrics(WinLoseMetricRequest request);
        Task<IXLWorksheet> CreateWinLoseMetricsWorksheet(
            XLWorkbook workbook,
            WinLoseMetricRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateWinLoseMetricsWorkbook(WinLoseMetricRequest request);
        Task<byte[]> GenerateWinLoseMetricsExcelFile(WinLoseMetricRequest request);
    }
}
