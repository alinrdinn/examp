using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.OLOPerformance.Data.Entities
{
    [Keyless]
    public class OLOPerformanceEntity
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }

        [Column("operator")]
        public string? @operator { get; set; }
        public string? platform { get; set; }
        public int? win { get; set; }
        public double? wow { get; set; }
        public string? remark { get; set; }
    }
}
