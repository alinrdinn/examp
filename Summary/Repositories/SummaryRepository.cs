using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.Summary.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.Summary.Repositories
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly ExecutiveDbContext _context;

        public SummaryRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<SummaryModel>> GetSummary(
            int? yearweek,
            string? level,
            string? location
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_total_tsel_win(@yearweek, @level, @location);";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
            };

            return await _context.Summaries.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<List<SummaryGapModel>> GetGapToOLO(
            int? yearweek,
            string? level,
            string? location,
            string? category
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_gap_to_olo(@yearweek, @level, @location, @category);";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
                new NpgsqlParameter("category", category ?? (object)DBNull.Value),
            };

            return await _context.SummaryGaps.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<List<SummaryGapModel>> GetWowFromWeek(
            int? yearweek,
            string? level,
            string? location,
            string? category
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_wow_week(@yearweek, @level, @location, @category);";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
                new NpgsqlParameter("category", category ?? (object)DBNull.Value),
            };

            return await _context.SummaryGaps.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<List<SummaryGapModel>> GetGapToH1(
            int? yearweek,
            string? level,
            string? location,
            string? category
        )
        {
            var sql =
                "SELECT * FROM semb.executive_dashboard_gap_comp(@yearweek, @level, @location, @category);";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("level", level ?? (object)DBNull.Value),
                new NpgsqlParameter("location", location ?? (object)DBNull.Value),
                new NpgsqlParameter("category", category ?? (object)DBNull.Value),
            };

            return await _context.SummaryGaps.FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}
