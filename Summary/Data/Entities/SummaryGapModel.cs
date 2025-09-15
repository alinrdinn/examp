using System;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.Summary.Data.Entities
{
    [Keyless]
    public class SummaryGapModel
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }
        public string? metric { get; set; }
        public double? percent { get; set; }
        public string? remark { get; set; }
    }
}
