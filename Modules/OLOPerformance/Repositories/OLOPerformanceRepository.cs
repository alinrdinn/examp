using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.OLOPerformance.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.OLOPerformance.Repositories
{
    public class OLOPerformanceRepository : IOLOPerformanceRepository
    {
        private readonly ExecutiveDbContext _context;

        public OLOPerformanceRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<OLOPerformanceEntity>> GetOloPerformance(
            int? yearweek,
            string? level,
            string? location
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_olo_performance_v2(@yearweek, @level, @location)";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("level", level),
                new NpgsqlParameter("location", location),
            };

            return await _context.OLOPerformances.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<List<OLOPerformanceSummaryEntity>> GetOloPerformanceSummary(
            int? yearweek,
            string? level,
            string? location
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_top_olo_performance(@yearweek, @level, @location);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
            };

            return await _context
                .Set<OLOPerformanceSummaryEntity>()
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }
    }
}
