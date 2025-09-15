using System;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Data.Entities
{
    [Keyless]
    public class WinLoseMetricEntity
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }
        public string? metric { get; set; }
        public double? score { get; set; }
        public double? wow { get; set; }
        public string? remark { get; set; }
        public int? win { get; set; }
        public double? percentage { get; set; }
        public int? increase { get; set; }
        public int? decrease { get; set; }
    }
}
