namespace backend_dtap.Modules.Qf.Executive.Application.DTOs.Responses
{

    // Prod and Subs Response Classes
    public class ExecProdResponse
    {
        public ExecKpiDetail? Revenue { get; set; }
        public ExecKpiDetail? Payload { get; set; }
        public ExecKpiDetail? Traffic { get; set; }
        public ExecKpiDetail? PayloadSc { get; set; }
    }

    public class ExecSubsResponse
    {
        public ExecKpiDetail? Rgb { get; set; }
        public ExecKpiDetail? Pgb { get; set; }
        public ExecKpiDetail? Vlr { get; set; }
        public ExecKpiDetail? PgbSc { get; set; }
    }

    public class ExecKpiDetail
    {
        public double? KpiValue { get; set; }
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public ExecKpiDetailMeasurement? Measurements { get; set; }
    }



    public class ExecKpiDetailMeasurement
    {
        public double? Yoy { get; set; }
        public double? Qoq { get; set; }
        public double? Mom { get; set; }
        public double? Wow { get; set; }
    }

    // Service Prod Category & Apps Response Classes
    public class ExecServiceProdCatResponse
    {
        public string? Category { get; set; }
        public double? Value { get; set; }
        public double? Percentage { get; set; }
    }


    public class ExecServiceProdAppResponse
    {
        public string? NameApps { get; set; }
        public double? Value { get; set; }
        public double? Payload { get; set; }
    }

    // Customer Exp Percieve Response Classes
    public class ExecCustExpPerceiveResponse
    {
        public ExecCustExpPerceiveDetail? Complain { get; set; }
        public ExecCustExpPerceiveDetail? NpsMobile { get; set; }
        public ExecCustExpPerceiveDetail? NpsFbb { get; set; }
        public ExecCustExpPerceiveDetail? Tpns { get; set; }
        public ExecCustExpPerceiveDetail? CsiNetwork { get; set; }
    }

    public class ExecCustExpPerceiveDetail
    {
        public double? Ach { get; set; }
        public string? TargetPeriod { get; set; }
        public string? TargetValue { get; set; }
        public string? Status { get; set; }
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
    }

    // Cust Exp Measure Response Classes


    public class ExecCustExpMeasureResponse
    {
        public ExecCustExpMeasureKQI? Kqi { get; set; }
        public ExecCustExpMeasureCeiDetail? Cei;
    }

    public class ExecCustExpMeasureKQI
    {
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }

        public ExecCustExpMeasureKQIDetail? WebBrowsingRtt { get; set; }
        public ExecCustExpMeasureKQIDetail? WebBrowsingUlRetransmit { get; set; }
        public ExecCustExpMeasureKQIDetail? GameUplinkJitter { get; set; }
        public ExecCustExpMeasureKQIDetail? VoipDownlinkJitter { get; set; }
        public ExecCustExpMeasureKQIDetail? XkbStartdelay { get; set; }
        public ExecCustExpMeasureKQIDetail? GameClientRtt { get; set; }
        public ExecCustExpMeasureKQIDetail? VoipUplinkJitter { get; set; }
    }


    public class ExecCustExpMeasureKQIDetail
    {
        public double? KpiValue { get; set; }
        public double? TotalBadCell { get; set; }
        public double? TotalCell { get; set; }
    }

    public class ExecCustExpMeasureCeiDetail
    {
        public double? KpiValue { get; set; }
        public string? TargetPeriod { get; set; }
        public string? TargetValue { get; set; }

        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public List<ExecSeries>? Series { get; set; }
    }
    
    public class ExecLocationSeries
    {
        public string? Location { get; set; }
        public List<ExecSeries>? Series { get; set; }
    }

    public class ExecSeries
    {
        public string? Name { get; set; }
        public double? Value { get; set; }
    }

    public class ExecServiceExpResponse
    {
        public ExecServiceExpResponseDetail? Web { get; set; }
        public ExecServiceExpResponseDetail? Game { get; set; }
        public ExecServiceExpResponseDetail? Video { get; set; }
        public ExecServiceExpResponseDetail? Im { get; set; }
        public ExecServiceExpResponseDetail? FileAccess { get; set; }
    }

    public class ExecServiceExpResponseDetail
    {
        public double? Ach { get; set; }
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public string? TargetPeriod { get; set; }
        public string? TargetValue { get; set; }

    }

    // Benchmark Response Classes
    public interface IExecBenchmarkResponse
    {
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
    }
    public class ExecBenchmarkOoklaResponse : IExecBenchmarkResponse
    {
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public List<ExecBenchmarkDetail>? GameScore { get; set; }
        public List<ExecBenchmarkDetail>? SpeedScore { get; set; }
        public List<ExecBenchmarkDetail>? VideoScore { get; set; }
        public List<ExecBenchmarkDetail>? CoverageScore { get; set; }
    }

    public class ExecBenchmarkOpensignalResponse : IExecBenchmarkResponse
    {
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public List<ExecBenchmarkDetail>? GameScore { get; set; }
        public List<ExecBenchmarkDetail>? DownloadSpeed { get; set; }
        public List<ExecBenchmarkDetail>? UploadSpeed { get; set; }
        public List<ExecBenchmarkDetail>? VideoExp { get; set; }
        public List<ExecBenchmarkDetail>? VoiceAppEx { get; set; }
        public List<ExecBenchmarkDetail>? Availability { get; set; }
        public List<ExecBenchmarkDetail>? ConsistencyQuality { get; set; }
        public List<ExecBenchmarkDetail>? CoverageScore { get; set; }
        public List<ExecBenchmarkDetail>? Reliability { get; set; }
    }

    public class ExecBenchmarkDetail
    {
        public string? OperatorName { get; set; }
        public double? KpiValue { get; set; }
        public int? Rank { get; set; }
        public string? Label { get; set; }
    }
    
    
    public class ExecNetResponse
    {
        public double? KpiValue { get; set; }
        public bool? IsUpdated { get; set; }
        public int? LastUpdated { get; set; }
        public string? TargetPeriod {get; set;}
        public string? TargetValue {get; set;}
        public ExecNetMeasurementDetail? Measurements { get; set; }
        public List<ExecNetLocationDetail>? Locations { get; set; }
        public List<ExecSeries>? Series { get; set; }
        public List<ExecNetMainKpiDetail>? MainKpi { get; set; }
    }
    
    public class ExecNetMeasurementDetail
    {
        public double? Wow { get; set; }
        public string? Status { get; set; }
    }
    public class ExecNetLocationDetail
    {
        public string? Location { get; set; }
        public double? KpiValue { get; set; }
        public double? Growth { get; set; }
        public string? Status { get; set; }
    }
    
    public class ExecNetMainKpiDetail
    {
        public string? Kpi { get; set; }
        public double? KpiValue { get; set; }
        public double? WowValue { get; set; }
    }

}
