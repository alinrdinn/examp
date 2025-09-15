using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Responses;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Services
{
    public interface IImproveDegradeService
    {
        Task<List<ImproveDegradeResponse>> GetImproveDegrade(ImproveDegradeRequest request);
        Task<XLWorkbook> GenerateWinLoseWorkbook(ImproveDegradeRequest request);
        Task<byte[]> GenerateWinLoseExcelFile(ImproveDegradeRequest request);
    }
}
