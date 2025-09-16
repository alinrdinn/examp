using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Responses;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Services
{
    public interface IImproveDegradeService
    {
        Task<List<NewImproveDegradeResponse>> GetImproveDegrade(ImproveDegradeRequest request);
        Task<IXLWorksheet> CreateWinLoseWorksheet(
            XLWorkbook workbook,
            ImproveDegradeRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateWinLoseWorkbook(ImproveDegradeRequest request);
        Task<byte[]> GenerateWinLoseExcelFile(ImproveDegradeRequest request);
    }
}
