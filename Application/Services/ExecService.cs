
using backend_dtap.Modules.Qf.Executive.Application.Interfaces;
using backend_dtap.Modules.Qf.Executive.Application.DTOs.Requests;
using backend_dtap.Modules.Qf.Executive.Application.DTOs.Responses;
using backend_dtap.Modules.Qf.Executive.Entites.Repositories;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace backend_dtap.Modules.Qf.Executive.Application.Services
{
    public class ExecService : IExecService
    {

        private readonly IExecRepo _repository;
        private readonly IMemoryCache _memoryCache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(6);
        public ExecService(IExecRepo repository, IMemoryCache memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        private static string ParseLocation(string input)
        {
            Match match = Regex.Match(input, @"^(R\d+)\s+(.+)$");
            if (match.Success)
            {
                return match.Groups[2].Value;
            }
            return input;
        }
        
        private string GenerateCacheKey(string functionName, object request)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            var jsonString = JsonConvert.SerializeObject(request, settings);

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
            var hashString = Convert.ToBase64String(hashBytes);

            return $"{functionName}_{hashString}";
        }

        public async Task<ExecProdResponse?> GetProdsWithStatus(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_ff484_GetProdsWithStatus", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecProdResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache

            var kpis = new List<string> { "revenue_total_bn", "payload_pb", "traffic_mn", "payload_sc" };
            var response = new ExecProdResponse();

            var location = ParseLocation(request.Location.ToUpper());

            var allData = await _repository.GetProdsSubsWithStatus(kpis, int.Parse(request.Time), request.Level.ToLower(), location);

            foreach (var data in allData)
            {
                var kpiDetail = new ExecKpiDetail
                {
                    KpiValue = data.value_kpi.HasValue ? Math.Round(data.value_kpi.Value, 2) : null,
                    IsUpdated = data.status == "updated",
                    LastUpdated = data.max_week,
                    Measurements = new ExecKpiDetailMeasurement
                    {
                        Yoy = data.yoy.HasValue ? Math.Round(data.yoy.Value, 2) : null,
                        Qoq = data.qoq.HasValue ? Math.Round(data.qoq.Value, 2) : null,
                        Mom = data.mom.HasValue ? Math.Round(data.mom.Value, 2) : null,
                        Wow = data.wow.HasValue ? Math.Round(data.wow.Value, 2) : null
                    }
                };

                switch (data.kpi.ToLower())
                {
                    case "revenue_total_bn":
                        response.Revenue = kpiDetail;
                        break;
                    case "payload_pb":
                        response.Payload = kpiDetail;
                        break;
                    case "traffic_mn":
                        response.Traffic = kpiDetail;
                        break;
                    case "payload_sc":
                        response.PayloadSc = kpiDetail;
                        break;
                }
            }

            
            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<ExecSubsResponse?> GetSubsWithStatus(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_ff4799_GetSubsWithStatus", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecSubsResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache

            var kpis = new List<string> { "rgb", "pgb", "vlr", "pgb_sc" };
            var response = new ExecSubsResponse();

            var location = ParseLocation(request.Location.ToUpper());

            var allData = await _repository.GetProdsSubsWithStatus(kpis, int.Parse(request.Time), request.Level.ToLower(), location);

            foreach (var data in allData)
            {
                var kpiDetail = new ExecKpiDetail
                {
                    KpiValue = data.value_kpi.HasValue ? Math.Round(data.value_kpi.Value,2) : null,
                    IsUpdated = data.status == "updated",
                    LastUpdated = data.max_week,
                    Measurements = new ExecKpiDetailMeasurement
                    {
                        Yoy = data.yoy.HasValue ? Math.Round(data.yoy.Value,2) : null,
                        Qoq = data.qoq.HasValue ? Math.Round(data.qoq.Value,2) : null,
                        Mom = data.mom.HasValue ? Math.Round(data.mom.Value,2) : null,
                        Wow = data.wow.HasValue ? Math.Round(data.wow.Value,2) : null
                    }
                };

                switch (data.kpi.ToLower())
                {
                    case "rgb":
                        response.Rgb = kpiDetail;
                        break;
                    case "pgb":
                        response.Pgb = kpiDetail;
                        break;
                    case "vlr":
                        response.Vlr = kpiDetail;
                        break;
                    case "payload_erl":
                        response.PgbSc = kpiDetail;
                        break;
                }
            }

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<List<ExecServiceProdCatResponse>?> GetServiceProdCat(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_ff4344_GetServiceProdCat", request);

            if (_memoryCache.TryGetValue(cacheKey, out List<ExecServiceProdCatResponse>? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache

            string category = "service";

            var location = ParseLocation(request.Location.ToUpper());
            location = location == "NATION" ? "nation" : location;

            var kpiMappings = new Dictionary<string, string>
            {
                { "im", "Instant Messaging" },
                { "streaming", "Streaming" },
                { "web_browsing", "Web Browsing" },
                { "sns", "Social Networking" },
                { "game", "Games" }
            };

            var data = await _repository.GetServiceProd(int.Parse(request.Time), category, request.Level.ToLower(), location);

            var response = kpiMappings.Select(kpi => new ExecServiceProdCatResponse
            {
                Category = kpi.Value,
                Value = null,
                Percentage = null
            }).ToList();

            if (data != null && data.Any())
            {
                foreach (var d in data)
                {
                    if (kpiMappings.ContainsKey(d.app_name.ToLower()))
                    {
                        var responseItem = response.First(r => r.Category == kpiMappings[d.app_name.ToLower()]);
                        responseItem.Value = d.trafficmb.HasValue ? Math.Round(d.trafficmb.Value, 2) : null;
                        responseItem.Percentage = d.wow.HasValue ? Math.Round(d.wow.Value, 2) : null;
                    }
                }
            }

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<List<ExecServiceProdAppResponse>?> GetServiceProdApps(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_ff744_GetServiceProdApps", request);

            if (_memoryCache.TryGetValue(cacheKey, out List<ExecServiceProdAppResponse>? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            string category = "app_name";

            var location = ParseLocation(request.Location.ToUpper());

            location = location == "NATION" ? "nation" : location;

            var data = await _repository.GetServiceProd(int.Parse(request.Time), category, request.Level.ToLower(), location);

            var response = data.Select(d => new ExecServiceProdAppResponse
            {
                NameApps = d.app_name,
                Payload = d.trafficmb.HasValue ? Math.Round(d.trafficmb.Value, 2) : null,
                Value = d.wow.HasValue ? Math.Round(d.wow.Value, 2) : null
            }).ToList();

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<ExecCustExpPerceiveResponse?> GetCustExpPerceive(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_fa2134_GetCustExpPerceive", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecCustExpPerceiveResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            var location = ParseLocation(request.Location.ToUpper());
            if (location == "NATION")
            {
                location = "nation";
            }

            var combinedData = await _repository.GetCustExpPerceives(
                int.Parse(request.Time),
                request.Level.ToLower(),
                location
            );

            var response = new ExecCustExpPerceiveResponse();

            if (combinedData == null || !combinedData.Any())
            {
                return response;
            }
            foreach (var item in combinedData)
            {
                var detail = new ExecCustExpPerceiveDetail
                {
                    Ach = item.achievement,
                    IsUpdated = item.status == "updated",
                    LastUpdated = item.last_updated,
                    Status = item.remark,
                    TargetPeriod = item.target_period,
                    TargetValue = item.target_value
                };

                switch (item.kpi?.ToLower())
                {
                    case "nps_mobile":
                        response.NpsMobile = detail;
                        break;
                    case "csi":
                        response.CsiNetwork = detail;
                        break;
                    case "tnps":
                        response.Tpns = detail;
                        break;
                    case "nps_fbb":
                        response.NpsFbb = detail;
                        break;
                    case "complain":
                        response.Complain = detail;
                        break;
                }
            }
            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<ExecCustExpMeasureResponse?> GetCustExpMeasure(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_f12b4_GetCustExpMeasure", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecCustExpMeasureResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            var response = new ExecCustExpMeasureResponse();
            var kqiResponse = new ExecCustExpMeasureKQI();
            var ceiResponse = new ExecCustExpMeasureCeiDetail();

            var location = ParseLocation(request.Location.ToUpper());
            var yearweek = int.Parse(request.Time);

            var kqiData = await _repository.GetCustExpMeasureKQI(yearweek, request.Level, location);

            if (kqiData != null && kqiData.Any())
            {
                var firstKqi = kqiData.First();
                kqiResponse.IsUpdated = firstKqi.status == "updated";
                kqiResponse.LastUpdated = firstKqi.max_week;

                foreach (var kqi in kqiData)
                {
                    var detail = new ExecCustExpMeasureKQIDetail
                    {
                        KpiValue = kqi.kpi_value.HasValue ? Math.Round(kqi.kpi_value.Value, 2) : null,
                        TotalBadCell = kqi.total_bad_cell.HasValue ? Math.Round(kqi.total_bad_cell.Value, 2) : null,
                        TotalCell = kqi.total_cell.HasValue ? Math.Round(kqi.total_cell.Value, 2) : null
                    };

                    switch (kqi.kpi_name?.ToLower())
                    {
                        case "game_client_rtt": kqiResponse.GameClientRtt = detail; break;
                        case "game_uplink_jitter": kqiResponse.GameUplinkJitter = detail; break;
                        case "voip_downlink_jitter": kqiResponse.VoipDownlinkJitter = detail; break;
                        case "voip_uplink_jitter": kqiResponse.VoipUplinkJitter = detail; break;
                        case "web_browsing_rtt": kqiResponse.WebBrowsingRtt = detail; break;
                        case "web_browsing_ul_retransmit": kqiResponse.WebBrowsingUlRetransmit = detail; break;
                        case "xkb_startdelay": kqiResponse.XkbStartdelay = detail; break;
                    }
                }
            }

            var ceiData = await _repository.GetCustExpMeasureCei(yearweek, request.Level, location);

            if (ceiData != null && ceiData.Any())
            {
                var firstCei = ceiData.Where(x => int.Parse(x.time) == yearweek).FirstOrDefault();

                ceiResponse.KpiValue = ceiData.FirstOrDefault(s => s.time == request.Time)?.value;
                ceiResponse.IsUpdated = firstCei?.status == "updated";
                ceiResponse.LastUpdated = firstCei?.max_week;
                ceiResponse.TargetValue = firstCei?.target;
                ceiResponse.TargetPeriod = firstCei?.target_period;

                ceiResponse.Series = ceiData.Select(s => new ExecSeries
                {
                    Name = s.time,
                    Value = s.value.HasValue ? Math.Round(s.value.Value, 2) : null,
                }).ToList();
            }

            response.Kqi = kqiResponse;
            response.Cei = ceiResponse;

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<ExecServiceExpResponse?> GetExecServiceExp(ExecRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_f131b4_GetExecServiceExp", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecServiceExpResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            var location = ParseLocation(request.Location.ToUpper());
            location = location == "NATION" ? "nation" : location;

            var response = new ExecServiceExpResponse();

            var data = await _repository.GetServiceExp(int.Parse(request.Time), request.Level.ToLower(), location);

            if (data == null)
            {
                return response;
            }

            response.Web = new ExecServiceExpResponseDetail
            {
                Ach = data.ach_web.HasValue ? Math.Round(data.ach_web.Value, 2) : null,
                IsUpdated = data.web_status == "updated",
                LastUpdated = data.web_max_week,
                TargetPeriod = data.quarter_name,
                TargetValue = data.web_target,
            };
            response.Game = new ExecServiceExpResponseDetail
            {
                Ach = data.ach_games.HasValue ? Math.Round(data.ach_games.Value, 2) : null,
                IsUpdated = data.games_status == "updated",
                LastUpdated = data.games_max_week,
                TargetPeriod = data.quarter_name,
                TargetValue = data.games_target,
            };
            response.Video = new ExecServiceExpResponseDetail
            {
                Ach = data.ach_video.HasValue ? Math.Round(data.ach_video.Value, 2) : null,
                IsUpdated = data.video_status == "updated",
                LastUpdated = data.video_max_week,
                TargetPeriod = data.quarter_name,
                TargetValue = data.video_target,
            };
            response.Im = new ExecServiceExpResponseDetail
            {
                Ach = data.ach_im.HasValue ? Math.Round(data.ach_im.Value, 2) : null,
                IsUpdated = data.im_status == "updated",
                LastUpdated = data.im_max_week,
                TargetPeriod = data.quarter_name,
                TargetValue = data.im_target,
            };
            response.FileAccess = new ExecServiceExpResponseDetail
            {
                Ach = data.ach_file_access.HasValue ? Math.Round(data.ach_file_access.Value, 2) : null,
                IsUpdated = data.file_access_status == "updated",
                LastUpdated = data.file_access_max_week,
                TargetPeriod = data.quarter_name,
                TargetValue = data.file_access_target,
            };

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<IExecBenchmarkResponse?> GetExecBenchmark(ExecFeatureRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_f1aas2_GetExecBenchmark", request);

            if (_memoryCache.TryGetValue(cacheKey, out IExecBenchmarkResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            var location = ParseLocation(request.Location.ToUpper());
            var feature = request.Feature.ToLower();
            var time = int.Parse(request.Time);

            var combinedData = await _repository.GetBenchmark(time, request.Level.ToLower(), location, feature);

            var firstRow = combinedData.FirstOrDefault();
            bool isUpdated = firstRow?.status == "updated";
            int? lastUpdated = firstRow?.max_week;

            var operatorOrder = new List<string> { "telkomsel", "indosat", "xl", "smartfren", "3" };
            IExecBenchmarkResponse response;

            if (feature == "ookla")
            {
                var ooklaResponse = new ExecBenchmarkOoklaResponse
                {
                    IsUpdated = isUpdated,
                    LastUpdated = lastUpdated,
                    CoverageScore = null
                };

                var kpiMappings = new Dictionary<string, Action<List<ExecBenchmarkDetail>>>
                {
                    { "ookla_game_score",  details => ooklaResponse.GameScore = details },
                    { "ookla_speed_score", details => ooklaResponse.SpeedScore = details },
                    { "ookla_video_score", details => ooklaResponse.VideoScore = details }
                };
                foreach (var mapping in kpiMappings)
                {
                    var kpi = mapping.Key;

                    var kpiDetails = combinedData
                        .Where(d => d.kpi.Equals(kpi, StringComparison.OrdinalIgnoreCase))
                        .Select(d => new ExecBenchmarkDetail
                        {
                            OperatorName = d.operator_,
                            KpiValue = d.value_kpi.HasValue ? Math.Round(d.value_kpi.Value, 2) : null,
                            Rank = d.value_rank,
                            Label = d.wow > 0.0 ? "good" : "bad"
                        })
                        .OrderBy(d => d.Rank)
                        .ToList();

                    mapping.Value(kpiDetails);
                }
                response = ooklaResponse;
            }
            else
            {
                var openSignalResponse = new ExecBenchmarkOpensignalResponse
                {
                    IsUpdated = isUpdated,
                    LastUpdated = lastUpdated
                };

                var kpiMappings = new Dictionary<string, Action<List<ExecBenchmarkDetail>>>
                {
                    { "onx_game_score",         details => openSignalResponse.GameScore = details },
                    { "onx_download_speed",     details => openSignalResponse.DownloadSpeed = details },
                    { "onx_upload_speed",       details => openSignalResponse.UploadSpeed = details },
                    { "onx_video_score",        details => openSignalResponse.VideoExp = details },
                    { "onx_voiceapp_score",     details => openSignalResponse.VoiceAppEx = details },
                    { "onx_consistent_quality", details => openSignalResponse.ConsistencyQuality = details },
                    { "onx_coverage",           details => openSignalResponse.CoverageScore = details },
                    { "onx_reliability",        details => openSignalResponse.Reliability = details },
                    { "onx_availability",       details => openSignalResponse.Availability = details }
                };

                foreach (var mapping in kpiMappings)
                {
                    var kpi = mapping.Key;

                    var kpiDetails = combinedData
                        .Where(d => d.kpi.Equals(kpi, StringComparison.OrdinalIgnoreCase))
                        .Select(d => new ExecBenchmarkDetail
                        {
                            OperatorName = d.operator_,
                            KpiValue = d.value_kpi.HasValue ? Math.Round(d.value_kpi.Value, 2) : null,
                            Rank = d.value_rank,
                            Label = d.wow > 0.0 ? "good" : "bad"
                        })
                        .OrderBy(d => d.Rank)
                        .ToList();

                    mapping.Value(kpiDetails);
                }
                response = openSignalResponse;
            }

            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

        public async Task<ExecNetResponse?> GetExecNetwork(ExecFeatureRequest request)
        {
            //Add Start Cache
            string cacheKey = GenerateCacheKey("exec_f23s2_GetExecNetwork", request);

            if (_memoryCache.TryGetValue(cacheKey, out ExecNetResponse? cachedResponse))
            {
                return cachedResponse;
            }
            //End Start Cache
            var indexKpi = request.Feature;

            if (indexKpi == "mhi") {
                indexKpi = "nhi";
            }
            
            var location = ParseLocation(request.Location.ToUpper());
            var response = new ExecNetResponse();

            var data = await _repository.GetNetworkWithStatusTarget(
                indexKpi,
                int.Parse(request.Time),
                request.Level.ToLower(),
                location
            );
            
            if (data != null)
            {
                response = new ExecNetResponse
                {
                    KpiValue = data.value_kpi.HasValue ? Math.Round(data.value_kpi.Value, 2) : null,
                    IsUpdated = data.status == "updated",
                    LastUpdated = data.max_week,

                    TargetPeriod = data.quarter_name,
                    TargetValue = data.target,

                    Measurements = new ExecNetMeasurementDetail
                    {
                        Wow = data.wow_kpi.HasValue ? Math.Round(data.wow_kpi.Value, 2) : null,
                        Status = data.kpi_status
                    }
                };

                string subLevel = "";
                if (request.Level == "nation") {
                    subLevel = "region";
                } else if (request.Level == "region") {
                    subLevel = "kabupaten";
                }

                if (!string.IsNullOrEmpty(subLevel)) {
                    var locations = await _repository.GetNetworkTopLocations(
                        subLevel,
                        location,
                        int.Parse(request.Time),
                        indexKpi
                    );

                    response.Locations = locations.Select(l => new ExecNetLocationDetail {
                        Location = request.Level == "nation"? l.regional: l.kabupaten,
                        KpiValue = l.value_kpi.HasValue ? Math.Round(l.value_kpi.Value, 2) : null,
                        Growth = l.wow_kpi.HasValue ? Math.Round(l.wow_kpi.Value, 2) : null,
                        Status = l.kpi_status
                    }).ToList();
                }

                var series = await _repository.GetNetworkSeries(
                            indexKpi,
                            request.Level,
                            location,
                            data.start_yearweek.ToString(),
                            data.end_yearweek.ToString()
                        );

                response.Series = series.Select(s => new ExecSeries {
                    Name = s.time,
                    Value = s.value.HasValue ? Math.Round(s.value.Value, 2) : null,
                }).ToList();

                var subKpiList = await _repository.GetGetNetworkTopKpis(indexKpi, request.Level, location, int.Parse(request.Time));

                response.MainKpi = subKpiList.Select(s => new ExecNetMainKpiDetail {
                    Kpi = s.sub_kpi,
                    KpiValue = s.value_kpi.HasValue ? Math.Round(s.value_kpi.Value, 2) : null,
                    WowValue = s.wow_value.HasValue ? Math.Round(s.wow_value.Value, 2) : null 
                }).ToList();
            }
            
            //Add End Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration,
            };

            _memoryCache.Set(cacheKey, response, cacheEntryOptions);
            //Add End Cache

            return response;
        }

    }
}