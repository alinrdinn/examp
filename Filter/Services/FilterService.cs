using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.Filter.Dtos.Requests;
using ExecutiveDashboard.Modules.Filter.Dtos.Responses;
using ExecutiveDashboard.Modules.Filter.Repositories;

namespace ExecutiveDashboard.Modules.Filter.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _repository;

        public FilterService(IFilterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FilterResponse>> GetYearweeks()
        {
            var rows = await _repository.GetYearweeks();

            var result = new List<FilterResponse>();
            foreach (var r in rows)
            {
                if (!r.yearweek.HasValue)
                    continue;

                var raw = r.yearweek.Value.ToString().PadLeft(6, '0');
                var yyyy = raw.Substring(0, 4);
                var ww = raw.Substring(4, 2);

                result.Add(new FilterResponse { Label = $"{yyyy}-{ww}", Value = $"{yyyy}{ww}" });
            }

            return result;
        }

        public async Task<List<FilterResponse>> GetLocations(FilterLocationRequest request)
        {
            var level = (request.Level ?? "region").Trim().ToLowerInvariant();

            if (level == "nation")
            {
                return new List<FilterResponse>
                {
                    new FilterResponse { Label = "NATIONWIDE", Value = "NATIONWIDE" },
                };
            }

            if (level == "kabupaten")
            {
                var cities = await _repository.GetCities();
                var resp = new List<FilterResponse>();
                foreach (var c in cities)
                {
                    if (string.IsNullOrWhiteSpace(c.city))
                        continue;
                    var v = c.city.Trim();
                    resp.Add(new FilterResponse { Label = c.city, Value = c.city });
                }
                return resp;
            }

            var regions = await _repository.GetRegions();
            var result = new List<FilterResponse>();
            foreach (var rg in regions)
            {
                if (string.IsNullOrWhiteSpace(rg.region))
                    continue;
                result.Add(new FilterResponse { Label = rg.region, Value = rg.region });
            }
            return result;
        }

        public async Task<List<FilterResponse>> GetCitiesByRegion(FilterRegionRequest request)
        {
            var region = request.Region?.Trim() ?? string.Empty;
            var rows = await _repository.GetCitiesByRegion(region);

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var result = new List<FilterResponse>();
            foreach (var r in rows)
            {
                if (string.IsNullOrWhiteSpace(r.city))
                    continue;
                var v = r.city.Trim();
                result.Add(new FilterResponse { Label = v, Value = v });
            }
            return result;
        }
    }
}
