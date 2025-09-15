using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses
{
    public class OLOPerformanceResponse
    {
        public string? OperatorWin { get; set; }
        public SourceWin? SourceWin { get; set; }
        public List<OtherOperatorItem>? OtherOperator { get; set; }
    }

    public class SourceWin
    {
        public PlatformWin? OpenSignalWin { get; set; }
        public PlatformWin? OoklaWin { get; set; }
    }

    public class PlatformWin
    {
        public int? Score { get; set; }
        public string? Wow { get; set; }
        public bool? Status { get; set; }
    }

    public class OtherOperatorItem
    {
        public string? Operator { get; set; }
        public int? Os { get; set; }
        public int? Ookla { get; set; }
    }
}
