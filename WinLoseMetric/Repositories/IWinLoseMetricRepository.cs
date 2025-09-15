
using ExecutiveDashboard.Modules.WinLoseMetric.Data.Entities;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Repositories
{
    public interface IWinLoseMetricRepository
    {
        Task<List<WinLoseMetricEntity>> GetWinLoseMetrics(
            int? yearweek,
            string? level,
            string? location,
            string? source,
            string? status
        );
    }
}
