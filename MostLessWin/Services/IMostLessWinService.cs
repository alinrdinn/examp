using ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Responses;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.MostLessWin.Services
{
    public interface IMostLessWinService
    {
        Task<MostLessWinResponse> GetMostLessWin(MostLessWinRequest request);
        Task<XLWorkbook> GenerateWinLoseMetricsWorkbook(MostLessWinRequest request);
        Task<byte[]> GenerateWinLoseMetricsExcelFile(MostLessWinRequest request);
    }
}
