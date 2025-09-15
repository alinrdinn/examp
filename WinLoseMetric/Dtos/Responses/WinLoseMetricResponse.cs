using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Responses
{
    public class WinLoseMetricResponse
    {
        public double? Percentage { get; set; }
        public int? Total { get; set; }
        public int? Increase { get; set; }
        public int? Decrease { get; set; }
        public List<MetricItem> Metrics { get; set; } = new();
    }

    public class MetricItem
    {
        public string? Title { get; set; }
        public double? Value { get; set; }
        public bool Status { get; set; }
        public double? Wow { get; set; }
    }
}
