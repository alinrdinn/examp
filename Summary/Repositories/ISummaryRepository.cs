using ExecutiveDashboard.Modules.Summary.Data.Entities;

namespace ExecutiveDashboard.Modules.Summary.Repositories
{
    public interface ISummaryRepository
    {
        Task<List<SummaryModel>> GetSummary(int? yearweek, string? level, string? location);
        Task<List<SummaryGapModel>> GetGapToOLO(
            int? yearweek,
            string? level,
            string? location,
            string? category
        );
        Task<List<SummaryGapModel>> GetWowFromWeek(
            int? yearweek,
            string? level,
            string? location,
            string? category
        );
        Task<List<SummaryGapModel>> GetGapToH1(
            int? yearweek,
            string? level,
            string? location,
            string? category
        );
    }
}
