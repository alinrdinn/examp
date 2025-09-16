using System.Collections.Generic;
using System.Threading.Tasks;
using ExecutiveDashboard.Modules.Filter.Data.Entities;

namespace ExecutiveDashboard.Modules.Filter.Repositories
{
    public interface IFilterRepository
    {
        Task<List<WeekReference>> GetYearweeks();
        Task<List<RegionReference>> GetRegions();
        Task<List<CityReference>> GetCities();
        Task<List<CityReference>> GetCitiesByRegion(string region);
    }
}
