using ClosedXML.Excel;
using ExecutiveDashboard.Modules.Summary.Dtos.Requests;
using ExecutiveDashboard.Modules.Summary.Dtos.Responses;

namespace ExecutiveDashboard.Modules.Summary.Services
{
    public interface ISummaryService
    {
        Task<SummaryResponse> GetSummary(SummaryRequest request);
        Task<IXLWorksheet> CreateSummaryWorksheet(
            XLWorkbook workbook,
            SummaryRequest request,
            string? worksheetName = null
        );
        Task<XLWorkbook> GenerateSummaryWorkbook(SummaryRequest request);
        Task<byte[]> GenerateSummaryExcelFile(SummaryRequest request);
    }
}
