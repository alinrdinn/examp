using ExecutiveDashboard.Modules.Summary.Dtos.Requests;
using ExecutiveDashboard.Modules.Summary.Dtos.Responses;

namespace ExecutiveDashboard.Modules.Summary.Services
{
    public interface ISummaryService
    {
        Task<SummaryResponse> GetSummary(SummaryRequest request);
        Task<byte[]> GenerateSummaryExcelFile(SummaryRequest request);
    }
}
