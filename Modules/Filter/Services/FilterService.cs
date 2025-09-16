using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.Filter.Dtos.Requests;
using ExecutiveDashboard.Modules.Filter.Dtos.Responses;
using ExecutiveDashboard.Modules.Filter.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace ExecutiveDashboard.Modules.Filter.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _repository;
        private readonly IMemoryCache _cache;

        public FilterService(IFilterRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<FilterResponse>> GetYearweeks()
        {
            const string cacheKey = "Filter_GetYearweeks";
            
            if (_cache.TryGetValue(cacheKey, out List<FilterResponse> cachedResult))
            {
                return cachedResult;
            }

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

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<List<FilterResponse>> GetLocations(FilterLocationRequest request)
        {
            var level = (request.Level ?? "region").Trim().ToLowerInvariant();
            var cacheKey = $"Filter_GetLocations_{level}";

            if (_cache.TryGetValue(cacheKey, out List<FilterResponse> cachedResult))
            {
                return cachedResult;
            }

            List<FilterResponse> result;

            if (level == "nation")
            {
                result = new List<FilterResponse>
                {
                    new FilterResponse { Label = "NATIONWIDE", Value = "NATIONWIDE" },
                };
            }
            else if (level == "kabupaten")
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
                result = resp;
            }
            else
            {
                var regions = await _repository.GetRegions();
                result = new List<FilterResponse>();
                foreach (var rg in regions)
                {
                    if (string.IsNullOrWhiteSpace(rg.region))
                        continue;
                    result.Add(new FilterResponse { Label = rg.region, Value = rg.region });
                }
            }

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<List<FilterResponse>> GetCitiesByRegion(FilterRegionRequest request)
        {
            var region = request.Region?.Trim() ?? string.Empty;
            var cacheKey = $"Filter_GetCitiesByRegion_{region}";

            if (_cache.TryGetValue(cacheKey, out List<FilterResponse> cachedResult))
            {
                return cachedResult;
            }

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

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
