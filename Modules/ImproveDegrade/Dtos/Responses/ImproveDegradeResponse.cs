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


    public class MaintainGroup
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
        public string? Status { get; set; }
        public ImproveGroup Improve { get; set; } = new();
        public ImproveGroup Degrade { get; set; } = new();
        public ImproveGroup Maintain { get; set; } = new();
    }

    public class NewRegionItem
    {
        public string? Name { get; set; }
        public List<string>? Details { get; set; }
        public string? Status { get; set; }
        
        public string? Remark { get; set; }
    }

    public class NewImproveDegradeResponse
    {
        public string? Location { get; set; }
        public int Score { get; set; }
        public int Lose { get; set; }
        public double? Percentage { get; set; }
        public string? Status { get; set; }
        public ImproveGroup Improve { get; set; } = new();
        public ImproveGroup Degrade { get; set; } = new();
        public ImproveGroup Maintain { get; set; } = new();
        public required List<NewRegionItem> Regions { get; set; }

    }
}
