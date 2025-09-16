using ClosedXML.Excel;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Responses;

namespace ExecutiveDashboard.Modules.MostLessWin.Services
{
    public interface IMostLessWinService
    {
        Task<MostLessWinResponse> GetMostLessWin(MostLessWinRequest request);
        Task<IXLWorksheet> CreateWinLoseMetricsWorksheet(
            XLWorkbook workbook,
            MostLessWinRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateWinLoseMetricsWorkbook(MostLessWinRequest request);
        Task<byte[]> GenerateWinLoseMetricsExcelFile(MostLessWinRequest request);
    }
}
