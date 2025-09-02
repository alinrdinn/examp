using Microsoft.EntityFrameworkCore;
using Npgsql;
using backend_dtap.Modules.Base.Infrastructure;
using backend_dtap.Modules.Qf.Executive.Entites.Repositories;
using backend_dtap.Modules.Qf.Executive.Domain.Entities;

namespace backend_dtap.Modules.Qf.Executive.Infrastructure.Repositories
{
    public class ExecRepo : IExecRepo
    {
        private readonly AppDbContext _context;

        public ExecRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExecProdWithStatus>> GetProdsSubsWithStatus(List<string> kpis, int time, string level, string location)
        {
            var kpisParam = new NpgsqlParameter("kpis", kpis.Select(k => k.ToLower()).ToArray());
            var timeParam = new NpgsqlParameter("time", time);
            var levelParam = new NpgsqlParameter("level", level);
            var locationParam = new NpgsqlParameter("location", location);

            var query = @"
                WITH prod_data AS (
                    SELECT *
                    FROM dtap.f_production_summary_card(@time, @level, @location)
                    WHERE LOWER(kpi) = ANY(@kpis)
                ),
                status_data AS (
                    SELECT *
                    FROM quality_framework.get_status_data(@time)
                )
                SELECT
                    p.kpi, p.level, p.location, p.value_kpi, p.value_kpi_mtd,
                    p.value_kpi_ytd, p.wow, p.mom, p.qoq, p.yoy,
                    s.status, s.max_week
                FROM prod_data p
                LEFT JOIN status_data s ON LOWER(p.kpi) = LOWER(s.kpi);
            ";

            return await _context.ExecProdsWithStatus
                .FromSqlRaw(query, timeParam, levelParam, locationParam, kpisParam)
                .ToListAsync();
        }


        public async Task<List<ExecServiceProdCat>> GetServiceProd(int yearweek, string category, string level, string location)
        {
            var yearweekParam = new NpgsqlParameter("@yearweek", yearweek);
            var categoryParam = new NpgsqlParameter("@category", category);
            var levelParam = new NpgsqlParameter("@level", level);
            var locationParam = new NpgsqlParameter("@location", location);

            var query = @"
                SELECT *
                FROM quality_framework.payload_apps_card(@yearweek, @category, @level, @location)
            ";

            return await _context.ExecServiceProdCats
                .FromSqlRaw(query, yearweekParam, categoryParam, levelParam, locationParam)
                .ToListAsync();
        }

        public async Task<List<ExecCustExpPerceive>> GetCustExpPerceives(int yearweek, string level, string location)
        {
            var yearweekParam = new NpgsqlParameter("@yearweek", yearweek);
            var levelParam = new NpgsqlParameter("@level", level);
            var locationParam = new NpgsqlParameter("@location", location);

            // var query = @"
            //     WITH TimeInfo AS (
            //         SELECT 
            //             CASE EXTRACT(QUARTER FROM date::date)
            //                 WHEN 1 THEN 'Q1'
            //                 WHEN 2 THEN 'Q2'
            //                 WHEN 3 THEN 'Q3'
            //                 ELSE 'Q4'
            //             END AS quarter_name
            //         FROM public.dimdate
            //         WHERE yearcalendarweek = @yearweek
            //         LIMIT 1
            //     ),
            //     KpiData AS (
            //         SELECT *
            //         FROM quality_framework.customer_experience_perceive_comparison(@yearweek, @level, @location)
            //     ),
            //     StatusData AS (
            //         SELECT * FROM quality_framework.get_status_data(@yearweek)
            //     ),
            //     TargetData AS (
            //         SELECT kpi, target 
            //         FROM quality_framework.goals_target
            //         WHERE 
            //             UPPER(location) = UPPER(@location)
            //             AND quarter = (SELECT quarter_name FROM TimeInfo)
            //     )
            //     SELECT 
            //         kpi.category AS kpi,
            //         kpi.value AS achievement,
            //         kpi.remark AS remark,
            //         stat.status,
            //         stat.max_week AS last_updated,
            //         tgt.target AS target_value,
            //         (SELECT quarter_name FROM TimeInfo) AS target_period
            //     FROM KpiData kpi
            //     LEFT JOIN StatusData stat ON kpi.category = stat.kpi
            //     LEFT JOIN TargetData tgt ON kpi.category = tgt.kpi;
            // ";

            
            var query = @"select * from quality_framework.customer_experience_perceive_comparison_v2(@yearweek, @level, @location)";

            return await _context.ExecCustExpPerceives
                .FromSqlRaw(query, yearweekParam, levelParam, locationParam)
                .ToListAsync();
        }

        public async Task<List<ExecCustExpMeasureKqi>> GetCustExpMeasureKQI(int yearweek, string level, string location)
        {
            var yearweekParam = new NpgsqlParameter("@yearweek", yearweek);
            var levelParam = new NpgsqlParameter("@level", level);
            var locationParam = new NpgsqlParameter("@location", location);

            var query = @"
                WITH StatusData AS (
                    SELECT kpi, status, max_week 
                    FROM quality_framework.get_status_data(@yearweek)
                )
                SELECT 
                    kqi.kpi_name,
                    kqi.kpi_value,
                    kqi.total_bad_cell,
                    kqi.total_cell,
                    sd.status,
                    sd.max_week
                FROM quality_framework.summary_7kqi kqi
                LEFT JOIN StatusData sd ON sd.kpi = 'kqi'
                WHERE kqi.yearweek = (
                    SELECT CASE 
                        WHEN max_week <= @yearweek THEN max_week 
                        ELSE @yearweek 
                    END 
                    FROM (
                        SELECT max_week FROM quality_framework.monitoring_week WHERE kpi = 'kqi'
                    ) AS mw
                )
                AND LOWER(kqi.level) = LOWER(@level)
                AND LOWER(kqi.location) = LOWER(@location);
            ";

            return await _context.ExecCustExpMeasureKqis
                .FromSqlRaw(query, yearweekParam, levelParam, locationParam)
                .ToListAsync();
        }

        public async Task<List<ExecCustExpMeasureCei>> GetCustExpMeasureCei(int yearweek, string level, string location)
        {
            var yearweekParam = new NpgsqlParameter("@yearweek", yearweek);
            var levelParam = new NpgsqlParameter("@level", level);
            var locationParam = new NpgsqlParameter("@location", location);

            var query = @"
                WITH StatusData AS (
                    SELECT status, max_week
                    FROM quality_framework.get_status_data(@yearweek)
                    WHERE kpi = 'cei'
                )
                SELECT
                    value AS value,
                    yearweek::TEXT AS time,
                    target::TEXT AS target,
                    quartal AS target_period,
                    (SELECT status FROM StatusData) AS status,
                    (SELECT max_week FROM StatusData) AS max_week
                FROM quality_framework.func_cei_mobile(@yearweek, @level, @location);
            ";

            return await _context.ExecCustExpMeasureCei
                .FromSqlRaw(query, yearweekParam, levelParam, locationParam)
                .ToListAsync();
        }

        public async Task<ExecServiceExp?> GetServiceExp(int yearweek, string level, string location)
        {
            var yearweekParam = new NpgsqlParameter("@yearweek", yearweek);
            var levelParam = new NpgsqlParameter("@level", level);
            var locationParam = new NpgsqlParameter("@location", location);

            var query = @"
                WITH TimeInfo AS (
                    SELECT
                        CASE EXTRACT(QUARTER FROM date::date)
                            WHEN 1 THEN 'Q1'
                            WHEN 2 THEN 'Q2'
                            WHEN 3 THEN 'Q3'
                            ELSE 'Q4'
                        END AS quarter_name
                    FROM public.dimdate
                    WHERE yearcalendarweek = @yearweek
                    LIMIT 1
                ),
                KpiData AS (
                    SELECT *
                    FROM quality_framework.service_experience(@yearweek, @level, @location)
                ),
                StatusData AS (
                    SELECT
                        MAX(CASE WHEN kpi = 'web' THEN status END) as web_status,
                        MAX(CASE WHEN kpi = 'web' THEN max_week END) as web_max_week,
                        MAX(CASE WHEN kpi = 'games' THEN status END) as games_status,
                        MAX(CASE WHEN kpi = 'games' THEN max_week END) as games_max_week,
                        MAX(CASE WHEN kpi = 'video' THEN status END) as video_status,
                        MAX(CASE WHEN kpi = 'video' THEN max_week END) as video_max_week,
                        MAX(CASE WHEN kpi = 'im' THEN status END) as im_status,
                        MAX(CASE WHEN kpi = 'im' THEN max_week END) as im_max_week,
                        MAX(CASE WHEN kpi = 'file_access' THEN status END) as file_access_status,
                        MAX(CASE WHEN kpi = 'file_access' THEN max_week END) as file_access_max_week
                    FROM quality_framework.get_status_data(@yearweek)
                ),
                TargetData AS (
                    SELECT
                        MAX(CASE WHEN kpi = 'web' THEN target END) as web_target,
                        MAX(CASE WHEN kpi = 'games' THEN target END) as games_target,
                        MAX(CASE WHEN kpi = 'video' THEN target END) as video_target,
                        MAX(CASE WHEN kpi = 'im' THEN target END) as im_target,
                        MAX(CASE WHEN kpi = 'file_access' THEN target END) as file_access_target
                    FROM quality_framework.goals_target
                    WHERE
                        UPPER(location) = UPPER(@location)
                        AND quarter = (SELECT quarter_name FROM TimeInfo)
                )
                SELECT
                    k.*,
                    s.*,
                    t.*,
                    ti.quarter_name
                FROM
                    KpiData k
                CROSS JOIN
                    TimeInfo ti
                LEFT JOIN
                    StatusData s ON 1=1
                LEFT JOIN
                    TargetData t ON 1=1
            ";

            return await _context.ExecServiceExps
                .FromSqlRaw(query, yearweekParam, levelParam, locationParam)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ExecBenchmark>> GetBenchmark(int time, string level, string location, string feature)
        {
            var timeParam = new NpgsqlParameter("time", time);
            var levelParam = new NpgsqlParameter("level", level);
            var locationParam = new NpgsqlParameter("location", location);
            var featureParam = new NpgsqlParameter("feature", feature);

            string kpiPrefix = (feature == "open_signal" ? "onx" : feature) + "_%";
            var kpiPrefixParam = new NpgsqlParameter("kpi_prefix", kpiPrefix);

            var query = @"
                WITH KpiData AS (
                    SELECT 
                        kpi,
                        location,
                        operator AS operator_,
                        value_kpi,
                        value_rank,
                        wow
                    FROM dtap.f_mobile_benchmark_summary_card(@time, @level, @location)
                    WHERE kpi LIKE @kpi_prefix
                ),
                StatusData AS (
                    SELECT status, max_week 
                    FROM quality_framework.get_status_data(@time)
                    WHERE kpi = @feature
                )
                SELECT 
                    kd.kpi,
                    kd.location,
                    kd.operator_,
                    kd.value_kpi,
                    kd.value_rank,
                    kd.wow,
                    sd.status,
                    sd.max_week
                FROM KpiData kd
                LEFT JOIN StatusData sd ON 1=1
            ";

            return await _context.ExecBenchmarks
                .FromSqlRaw(query, timeParam, levelParam, locationParam, featureParam, kpiPrefixParam)
                .ToListAsync();
        }

        // public async Task<ExecNetworkWithStatusTarget?> GetNetworkWithStatusTarget(string kpi, int time, string level, string location)
        // {
        //     var kpiParam = new NpgsqlParameter("kpi", kpi);
        //     var timeParam = new NpgsqlParameter("time", time);
        //     var levelParam = new NpgsqlParameter("level", level);
        //     var locationParam = new NpgsqlParameter("location", location);

        //     var query = $@"
        //         WITH KpiData AS (
        //             SELECT *
        //             FROM quality_framework.network_service_hygiene_index_update_target_v2(@time)
        //             WHERE LOWER(category) = LOWER(@kpi)
        //             AND LOWER(level) = LOWER(@level)
        //             AND (
        //                     (LOWER(@level) <> 'kabupaten' AND LOWER(regional) = LOWER(@location))
        //                     OR
        //                     (LOWER(@level) = 'kabupaten' AND LOWER(kabupaten) = LOWER(@location))
        //             )
        //         ),
        //         StatusData AS (
        //             SELECT * FROM quality_framework.get_status_data(@time)
        //         ),

        //         p AS (
        //             SELECT
        //                 @time::int AS yearweek,
        //                 (@time::int / 100)::int AS yr,
        //                 /* quarter of the given yearweek (1–4) */
        //                 (
        //                 SELECT (d.yearquartal::int % 100)
        //                 FROM public.dimdate d
        //                 WHERE d.yearcalendarweek = @time::int
        //                 LIMIT 1
        //                 ) AS qtr_from_week,
        //                 (
        //                 SELECT MIN(d.yearcalendarweek)
        //                 FROM public.dimdate d
        //                 WHERE d.year = (@time::int / 100)::int
        //                 ) AS start_yearweek
        //             ),
        //         TimeInfo AS (
        //             SELECT
        //             COALESCE(q.quarter_name, 'Q' || p.qtr_from_week::text) AS quarter_name,
        //             p.start_yearweek,
        //             (
        //                 SELECT MAX(d.yearcalendarweek)
        //                 FROM public.dimdate d
        //                 WHERE d.year = p.yr
        //                 AND d.yearquartal::int = p.yr * 100
        //                     + COALESCE(
        //                         NULLIF(regexp_replace(q.quarter_name, '[^0-9]', '', 'g'), '')::int,
        //                         p.qtr_from_week
        //                     )
        //             ) AS end_yearweek
        //             FROM p
        //             CROSS JOIN (VALUES (NULL::text), ('Q1'), ('Q2'), ('Q3'), ('Q4')) AS q(quarter_name)
        //             ORDER BY q.quarter_name NULLS FIRST
        //         )
        //         SELECT
        //             kd.value_kpi,
        //             kd.wow_kpi,
        //             kd.kpi_status,
        //             kd.target_edit AS target,
        //             kd.quarter AS quarter_name,
        //             sd.status,
        //             sd.max_week,
        //             ti.start_yearweek,
        //             ti.end_yearweek
        //         FROM KpiData kd
        //         LEFT JOIN TimeInfo ti ON LOWER(kd.quarter) = LOWER(ti.quarter_name)
        //         LEFT JOIN StatusData sd ON LOWER(kd.category) = LOWER(sd.kpi)
        //     ";

        //     return await _context.ExecNetworkWithStatusTargets
        //         .FromSqlRaw(query, kpiParam, timeParam, levelParam, locationParam)
        //         .FirstOrDefaultAsync();
        // }

        public async Task<ExecNetworkWithStatusTarget?> GetNetworkWithStatusTarget(string kpi, int time, string level, string location)
        {
            var kpiParam = new NpgsqlParameter("kpi", kpi);
            var timeParam = new NpgsqlParameter("time", time);
            var levelParam = new NpgsqlParameter("level", level);
            var locationParam = new NpgsqlParameter("location", location);

            var query = $@"
                WITH TimeInfo AS (
                    SELECT
                        yearcalendarweek,
                        CASE EXTRACT(QUARTER FROM date::date)
                            WHEN 1 THEN 'Q1'
                            WHEN 2 THEN 'Q2'
                            WHEN 3 THEN 'Q3'
                            ELSE 'Q4'
                        END AS quarter_name,
                        
                        ((SELECT MIN(d2.yearcalendarweek) FROM public.dimdate d2 WHERE d2.year = d1.year AND d2.yearquartal = d1.yearquartal) / 100) * 100 + 1 AS start_yearweek,
                        (SELECT MAX(d2.yearcalendarweek) FROM public.dimdate d2 WHERE d2.year = d1.year AND d2.yearquartal = d1.yearquartal) AS end_yearweek
                    FROM public.dimdate d1
                    WHERE yearcalendarweek = @time
                    LIMIT 1
                ),
                KpiData AS (
                    SELECT *
                    FROM quality_framework.network_service_hygiene_index_update_target_v2(@time)
                    WHERE LOWER(category) = LOWER(@kpi)
                    AND LOWER(level) = LOWER(@level)
                    AND (
                            (LOWER(@level) <> 'kabupaten' AND LOWER(regional) = LOWER(@location))
                            OR
                            (LOWER(@level) = 'kabupaten' AND LOWER(kabupaten) = LOWER(@location))
                    )
                ),
                StatusData AS (
                    SELECT * FROM quality_framework.get_status_data(@time)
                ),
                TargetData AS (
                    SELECT kpi, target
                    FROM quality_framework.goals_target
                    WHERE
                        UPPER(location) = UPPER(@location)
                        AND quarter = (SELECT quarter_name FROM TimeInfo)
                )
                SELECT
                    kd.value_kpi,
                    kd.wow_kpi,
                    kd.kpi_status,
                    kd.target_edit::TEXT AS target,
                    kd.quarter AS quarter_name,
                    sd.status,
                    sd.max_week,
                    ti.start_yearweek,
                    ti.end_yearweek
                FROM KpiData kd
                CROSS JOIN TimeInfo ti
                LEFT JOIN StatusData sd ON LOWER(kd.category) = LOWER(sd.kpi)
                LEFT JOIN TargetData td ON LOWER(kd.category) = LOWER(td.kpi)
            ";

            return await _context.ExecNetworkWithStatusTargets
                .FromSqlRaw(query, kpiParam, timeParam, levelParam, locationParam)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ExecNetworkWithRegional>> GetNetworkTopLocations(string geographicLevel, string locationName, int timeFrame, string metricCategory)
        {
            var geographicLevelParam = new NpgsqlParameter("geographicLevel", geographicLevel);
            var timeFrameParam = new NpgsqlParameter("timeFrame", timeFrame);
            var metricCategoryParam = new NpgsqlParameter("metricCategory", metricCategory);

            var locationWhereClause = "";

            if (geographicLevel.ToLower() == "kabupaten")
            {
                locationWhereClause = $"AND UPPER(regional) = '{locationName}'";
            }

            var ascOrDesc = "ASC";
            if (metricCategory == "rci")
            {
                ascOrDesc = "DESC";
            }

            var query = $@"
                SELECT 
                    value_kpi,
                    wow_kpi,
                    kpi_status,
                    regional,
                    kabupaten

                FROM quality_framework.network_service_hygiene_index_update_target_v2(@timeFrame) s
                WHERE UPPER(level) = UPPER(@geographicLevel) {locationWhereClause}
                AND LOWER(category) = LOWER(@metricCategory)
                ORDER BY value_kpi {ascOrDesc}
                LIMIT 3;
            ";

            return await _context.ExecNetworkWithRegionals
                .FromSqlRaw(query, geographicLevelParam, timeFrameParam, metricCategoryParam)
                .ToListAsync();
        }

        public async Task<List<ExecNetworkTopKpi>> GetGetNetworkTopKpis(string kpi, string level, string locationName, int yearweek)
        {
            var kpiParam = new NpgsqlParameter("kpi", kpi);
            var levelParam = new NpgsqlParameter("level", level);
            var locationNameParam = new NpgsqlParameter("locationName", locationName);
            var yearweekParam = new NpgsqlParameter("yearweek", yearweek);

            var query = $@"
                WITH latest_week AS (
                    SELECT 
                        CASE 
                            WHEN max_week <= @yearweek::INT THEN max_week 
                            ELSE @yearweek::INT
                        END AS latest_available_week
                    FROM (
                        SELECT MAX(max_week) AS max_week 
                        FROM quality_framework.monitoring_week 
                        WHERE kpi = @kpi
                    ) mw
                )
                SELECT
                    *
                FROM 
                    quality_framework.sub_kpi_network
                WHERE
                    level = @level
                    AND UPPER(location) = UPPER(@locationName)  
                    AND week = (SELECT latest_available_week FROM latest_week)
                    AND main_kpi = @kpi;

            ";

            return await _context.ExecNetworkTopKpis
                .FromSqlRaw(query, kpiParam, levelParam, locationNameParam, yearweekParam)
                .ToListAsync();
        }
        
        public async Task<List<ExecSeriesEntity>> GetNetworkSeries(string kpi, string level, string locationName, string startYearweek, string endYearweek)
        {
            var levelParam = new NpgsqlParameter("level", level);
            var locationNameParam = new NpgsqlParameter("locationName", locationName);
            var kpiParam = new NpgsqlParameter("kpi", kpi);
            var startYearweekParam = new NpgsqlParameter("startYearweek", startYearweek);
            var endYearweekParam = new NpgsqlParameter("endYearweek", endYearweek);

            var query = $@"
                SELECT 
                    'actual' AS type, 
                    yearweek::TEXT AS time,
                    ach AS value 
                FROM 
                    quality_framework.summary_trend_executive
                WHERE 
                    level = @level
                    AND UPPER(location) = UPPER(@locationName)
                    AND KPI = @kpi
                    AND yearweek >= @startYearweek::INT
                    AND yearweek <= @endYearweek::INT
                ORDER BY yearweek;
            ";

            return await _context.ExecSeries
                .FromSqlRaw(query, levelParam, locationNameParam, kpiParam, startYearweekParam, endYearweekParam)
                .ToListAsync();
        }
        

    }
}