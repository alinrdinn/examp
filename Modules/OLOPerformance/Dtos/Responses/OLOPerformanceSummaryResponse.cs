using System.Collections.Generic;

namespace ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses
{
    public class OLOPerformanceMetric
    {
        public string? Title { get; set; }
        public double? Score { get; set; }
        public string? Wow { get; set; }
        public string? GapWithTelkomsel { get; set; }
        public string? StatusWow { get; set; }
        public string? StatusGap { get; set; }
    }

    public class OLOPerformancePlatformSummary
    {
        public List<OLOPerformanceMetric> Metrics { get; set; }
        public List<string> Summary { get; set; }
    }

    public class OLOPerformanceSummaryResponse
    {
        public string? Operator { get; set; }
        public OLOPerformancePlatformSummary? OpenSignal { get; set; }
        public OLOPerformancePlatformSummary? Ookla { get; set; }
    }
}
