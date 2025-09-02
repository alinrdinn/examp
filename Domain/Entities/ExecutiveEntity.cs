using Microsoft.EntityFrameworkCore;

namespace backend_dtap.Modules.Qf.Executive.Domain.Entities
{

    // Productivity Card
    [Keyless]
    public class ExecProdWithStatus
    {
        public required string kpi { get; set; }
        public required string level { get; set; }
        public required string location { get; set; }
        public double? value_kpi { get; set; }
        public double? value_kpi_mtd { get; set; }
        public double? value_kpi_ytd { get; set; }
        public double? wow { get; set; }
        public double? mom { get; set; }
        public double? qoq { get; set; }
        public double? yoy { get; set; }

        public string? status { get; set; }
        public int? max_week { get; set; }
    }

    // Subscriber Card
    [Keyless]
    public class ExecSubs
    {
        public required string kpi { get; set; }
        public required string level { get; set; }
        public required string location { get; set; }
        public double? value_kpi { get; set; }
        public double? value_kpi_mtd { get; set; }
        public double? value_kpi_ytd { get; set; }
        public double? wow { get; set; }
        public double? mom { get; set; }
        public double? qoq { get; set; }
        public double? yoy { get; set; }
    }

    // Service Productivity Card

    [Keyless]
    public class ExecServiceProdCat
    {
        public required string app_name { get; set; }
        public double? trafficmb { get; set; }
        public double? wow { get; set; }
    }

    // Customer Exp Perceive
    [Keyless]
    public class ExecCustExpPerceive
    {
        public string? kpi { get; set; }
        public double? achievement { get; set; }
        public string? status { get; set; }
        public string? remark { get; set; }
        public int? last_updated { get; set; }
        public string? target_value { get; set; }
        public string? target_period { get; set; }
    }

    // Cust Exp Measure
    [Keyless]
    public class ExecCustExpMeasureKqi
    {
        public string? kpi_name { get; set; }
        public double? kpi_value { get; set; }
        public double? total_bad_cell { get; set; }
        public double? total_cell { get; set; }
        public string? status { get; set; }
        public int? max_week { get; set; }
    }

    [Keyless]
    public class ExecCustExpMeasureCei
    {
        public required string time { get; set; }
        public double? value { get; set; }
        public string? status { get; set; }
        public int? max_week { get; set; }
        public string? target_period { get; set; }
        public string? target { get; set; }
    }

    [Keyless]
    public class ExecServiceExp
    {
        public double? ach_web { get; set; }
        public double? ach_games { get; set; }
        public double? ach_video { get; set; }
        public double? ach_im { get; set; }
        public double? ach_file_access { get; set; }

        public string? web_status { get; set; }
        public int? web_max_week { get; set; }
        public string? games_status { get; set; }
        public int? games_max_week { get; set; }
        public string? video_status { get; set; }
        public int? video_max_week { get; set; }
        public string? im_status { get; set; }
        public int? im_max_week { get; set; }
        public string? file_access_status { get; set; }
        public int? file_access_max_week { get; set; }

        public string? web_target { get; set; }
        public string? games_target { get; set; }
        public string? video_target { get; set; }
        public string? im_target { get; set; }
        public string? file_access_target { get; set; }

        public string? quarter_name { get; set; }
    }


    [Keyless]
    public class ExecBenchmark
    {
        public required string kpi { get; set; }
        public required string location { get; set; }
        public required string operator_ { get; set; }
        public double? value_kpi { get; set; }
        public int? value_rank { get; set; }
        public double? wow { get; set; }
        public string? status { get; set; }
        public int? max_week { get; set; }
    }

    [Keyless]
    public class ExecNetwork
    {
        public double? value_kpi { get; set; }
        public double? wow_kpi { get; set; }
        public string? kpi_status { get; set; }
    }


    [Keyless]
    public class ExecNetworkWithRegional : ExecNetwork
    {

        public string? regional { get; set; }
        public string? kabupaten { get; set; }
    }
    [Keyless]
    public class ExecNetworkWithStatusTarget : ExecNetwork
    {
        public string? status { get; set; }
        public int? max_week { get; set; }
        public string? target { get; set; }
        public string? quarter_name { get; set; }
        public int? start_yearweek { get; set; }
        public int? end_yearweek { get; set; }
    }



    [Keyless]
    public class ExecNetworkTopKpi
    {
        public string? sub_kpi { get; set; }
        public double? value_kpi { get; set; }
        public double? wow_value { get; set; }
    }
    
    
    [Keyless]
    public class ExecSeriesEntity
    {
        public required string type {get; set;}
        public required string time {get; set;}
        public double? value { get; set; }
    }
}