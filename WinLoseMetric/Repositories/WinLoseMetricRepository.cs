using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.WinLoseMetric.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Repositories
{
    public class WinLoseMetricRepository : IWinLoseMetricRepository
    {
        private readonly ExecutiveDbContext _context;

        public WinLoseMetricRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<WinLoseMetricEntity>> GetWinLoseMetrics(
            int? yearweek,
            string? level,
            string? location,
            string? source,
            string? status
        )
        {
            string fn = (
                source?.Trim().ToLowerInvariant(),
                status?.Trim().ToLowerInvariant()
            ) switch
            {
                ("ookla", "win") => "semb.executive_dashboard_tsel_ookla_win",
                ("ookla", "lose") => "semb.executive_dashboard_tsel_ookla_lose",
                ("open_signal", "win") => "semb.executive_dashboard_tsel_os_win",
                ("open_signal", "lose") => "semb.executive_dashboard_tsel_os_lose",
                _ => throw new ArgumentException("Invalid source or status"),
            };

            string cols = (
                source?.Trim().ToLowerInvariant(),
                status?.Trim().ToLowerInvariant()
            ) switch
            {
                ("ookla", "win") => "*",
                ("ookla", "lose") => "*, lose AS win",
                ("open_signal", "win") => "*",
                ("open_signal", "lose") => "*, lose AS win",
                _ => throw new ArgumentException("Invalid source or status"),
            };

            var sql = $"SELECT {cols} FROM {fn}(@yearweek, @level, @location);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek is null ? DBNull.Value : yearweek),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
            };

            return await _context.WinLoseMetrics.FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}
