using System;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.Summary.Data.Entities
{
    [Keyless]
    public class SummaryModel
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }
        public string? platform { get; set; }
        public int? win { get; set; }
        public int? lose { get; set; }
        public double? percent { get; set; }
    }
}
