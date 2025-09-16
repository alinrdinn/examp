using ExecutiveDashboard.Modules.OLOPerformance.Data.Entities;

namespace ExecutiveDashboard.Modules.OLOPerformance.Repositories
{
    public interface IOLOPerformanceRepository
    {
        Task<List<OLOPerformanceEntity>> GetOloPerformance(
            int? yearweek,
            string? level,
            string? location
        );

        Task<List<OLOPerformanceSummaryEntity>> GetOloPerformanceSummary(
            int? yearweek,
            string? level,
            string? location
        );
    }
}
