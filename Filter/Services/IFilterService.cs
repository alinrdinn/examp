using ExecutiveDashboard.Modules.Filter.Dtos.Requests;
using ExecutiveDashboard.Modules.Filter.Dtos.Responses;

namespace ExecutiveDashboard.Modules.Filter.Services
{
    public interface IFilterService
    {
        Task<List<FilterResponse>> GetYearweeks();
        Task<List<FilterResponse>> GetLocations(FilterLocationRequest request);
        Task<List<FilterResponse>> GetCitiesByRegion(FilterRegionRequest request);
    }
}
