using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses
{
    public class OLOPerformanceDetail
    {
        public string? Title { get; set; }
        public double? Score { get; set; }
        public string? Improvement { get; set; }
        public string? GapWithTelkomsel { get; set; }
    }

    public class OLOPerformanceSummaryResponse
    {
        public string? Operator { get; set; }
        public OLOPerformanceDetail? OpenSignal { get; set; }
        public OLOPerformanceDetail? Ookla { get; set; }
        public List<string>? Summary { get; set; }
    }
}
