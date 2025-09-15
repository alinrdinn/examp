using System.ComponentModel.DataAnnotations.Schema;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Responses
{
    
    public class RegionItem
    {
        public string? Name { get; set; }
        public List<string>? Details { get; set; }
        public string? Status { get; set; }
    }

    public class ImproveGroup
    {
        public int Total { get; set; }
        public List<RegionItem> Regions { get; set; } = new();
    }

    public class DegradeGroup
    {
        public int Total { get; set; }
        public List<RegionItem> Regions { get; set; } = new();
    }

    public class ImproveDegradeResponse
    {
        public string? Location { get; set; }
        public int Score { get; set; }
        public int Lose { get; set; }
        public double? Percentage { get; set; }
        public ImproveGroup Improve { get; set; } = new();
        public DegradeGroup Degrade { get; set; } = new();
    }
}
