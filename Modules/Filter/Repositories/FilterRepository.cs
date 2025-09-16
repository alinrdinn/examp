using ExecutiveDashboard.Common;
using ExecutiveDashboard.Modules.Filter.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExecutiveDashboard.Modules.Filter.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        private readonly ExecutiveDbContext _context;

        public FilterRepository(ExecutiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<WeekReference>> GetYearweeks()
        {
            var sql =
                @"
                SELECT DISTINCT yearweek
                FROM semb.reff_week_region_city
                WHERE yearweek IS NOT NULL
                ORDER BY yearweek ASC;";

            return await _context.WeekReferences.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<List<RegionReference>> GetRegions()
        {
            var sql =
                @"
                SELECT DISTINCT region
                FROM semb.reff_week_region_city
                WHERE region IS NOT NULL AND trim(region) <> ''
                ORDER BY region ASC;";

            return await _context.RegionReferences.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<List<CityReference>> GetCities()
        {
            var sql =
                @"
                SELECT DISTINCT city
                FROM semb.reff_week_region_city
                WHERE city IS NOT NULL AND trim(city) <> ''
                ORDER BY city ASC;";

            return await _context.CityReferences.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<List<CityReference>> GetCitiesByRegion(string region)
        {
            var sql =
                @"
                SELECT DISTINCT city
                FROM semb.reff_week_region_city
                WHERE region = @region
                  AND city IS NOT NULL AND trim(city) <> ''
                ORDER BY city ASC;";

            var parameters = new[] { new NpgsqlParameter("@region", region ?? string.Empty) };

            return await _context.CityReferences.FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}
