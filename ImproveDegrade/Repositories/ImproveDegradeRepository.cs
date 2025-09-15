using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Repositories
{
    public class ImproveDegradeRepository : IImproveDegradeRepository
    {
        private readonly ExecutiveDbContext _context;

        public ImproveDegradeRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocationWinLoseEntity>> GetAreaWinLose(int yearweek, string level, string location, string source)
        {
            
            source = (
                source
            ) switch
            {
                ("ookla") => "ookla",
                ("open_signal") => "os",
                _ => throw new ArgumentException("Invalid source or status"),
            };
            const string sql = @"select * from semb.executive_dashboard_area_win(@yearweek, @level, @location, @source);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("level", level),
                new NpgsqlParameter("location", location),
                new NpgsqlParameter("source", source),
            };

            return await _context
                .LocationWinLoses
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
