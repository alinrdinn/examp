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

        public async Task<List<LocationWinLoseEntity>> GetAreaWinLose(
            int yearweek,
            string source
        )
        {
            source = (source) switch
            {
                ("ookla") => "ookla",
                ("open_signal") => "os",
                _ => throw new ArgumentException("Invalid source or status"),
            };
            const string sql =
                @"select * from semb.executive_dashboard_area_win(@yearweek, 'nation', 'nationwide', @source);";

            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("source", source),
            };

            return await _context
                .LocationWinLoses.FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ImproveMaintainDegradeEntity>> GetRegionCityMappings(int yearweek, string source)
        {
            source = source switch
            {
                "ookla" => "ookla",
                "open_signal" => "os",
                _ => throw new ArgumentException("Invalid source or status")
            };

            const string sql = @"select * from semb.executive_dashboard_mapping_region_city(@yearweek, @source);";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("source", source),
            };

            return await _context
                .ImproveMaintainDegradeEntities
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }
        
        public async Task<List<AreaStatusEntity>> GetAreaStatusMappings(int yearweek, string source)
        {
            source = source switch
            {
                "ookla" => "ookla",
                "open_signal" => "os",
                _ => throw new ArgumentException("Invalid source or status")
            };

            const string sql = @"
            select distinct area, remark_area
            from semb.executive_dashboard_mapping_region_city(@yearweek, @source)
            where remark_area IS NOT NULL
            order by area, remark_area            
            ";
            var parameters = new[]
            {
                new NpgsqlParameter("yearweek", yearweek),
                new NpgsqlParameter("source", source),
            };

            return await _context
                .AreaStatusEntities
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }
    }
}
