using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.MostLessWin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.MostLessWin.Repositories
{
    public class MostLessWinRepository : IMostLessWinRepository
    {
        private readonly ExecutiveDbContext _context;

        public MostLessWinRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<MostLessWinEntity>> GetMostWinForLatestWeek(
            int yearweek,
            string level,
            string location,
            string platform
        )
        {
            platform = (platform) switch
            {
                "open_signal" => "os",
                _ => platform,
            };
            var sql =
                @"
                select * from semb.executive_dashboard_most_win(@yearweek, @level, @location)
                where platform = @platform;
            ";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("level", level),
                new NpgsqlParameter("location", location),
                new NpgsqlParameter("platform", platform),
            };

            return await _context.MostLessWins.FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}
