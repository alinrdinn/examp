using System;
using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.SummaryNote.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.SummaryNote.Repositories
{
    public class SummaryNoteRepository : ISummaryNoteRepository
    {
        private readonly ExecutiveDbContext _context;

        public SummaryNoteRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<SummaryNoteEntity>> GetSummaryNotes(int? yearweek)
        {
            const string sql = "select * from semb.get_executive_dashboard_summary_card(@yearweek);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
            };

            return await _context.SummaryNotes.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public Task CreateSummaryNote(int? yearweek, string? detail)
        {
            const string sql =
                "call semb.insert_executive_dashboard_summary_card(@yearweek, @detail);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("detail", detail ?? (object)DBNull.Value),
            };

            return _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public Task UpdateSummaryNote(int id, int? yearweek, string? detail)
        {
            const string sql =
                "call semb.update_to_executive_dashboard_summary_card(@yearweek, @id, @detail);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek ?? (object)DBNull.Value),
                new NpgsqlParameter("id", id),
                new NpgsqlParameter("detail", detail ?? (object)DBNull.Value),
            };

            return _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}
