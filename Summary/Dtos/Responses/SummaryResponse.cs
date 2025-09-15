namespace ExecutiveDashboard.Modules.Summary.Dtos.Responses
{
    public class SummaryResponse
    {
        public int? TotalTelkomselWin { get; set; }
        public double? PercentageTelkomselWin { get; set; }
        public int? TotalOSWin { get; set; }
        public double? PercentageOSWin { get; set; }
        public int? TotalOoklaWin { get; set; }
        public double? PercentageOoklaWin { get; set; }
        public int? TotalTelkomselLose { get; set; }
        public int? TotalOSLose { get; set; }
        public int? TotalOoklaLose { get; set; }

        public List<GapMetric> GapToOLO { get; set; } = new();
        public List<GapMetric> WowFromWeek { get; set; } = new();
        public List<GapMetric> GapToH1 { get; set; } = new();
    }

    public class GapMetric
    {
        public string? Category { get; set; }
        public MetricDetail Strong { get; set; } = new();
        public MetricDetail Weak { get; set; } = new();
    }

    public class MetricDetail
    {
        public string? Label { get; set; }
        public double? Value { get; set; }
    }
}
