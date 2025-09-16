using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.OLOPerformance.Data.Entities
{
    [Keyless]
    public class OLOPerformanceSummaryEntity
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }

        public string? metric { get; set; }

        [Column("operator")]
        public string? @operator { get; set; }
        public string? platform { get; set; }
        public double? value { get; set; }
        public double? gap_telkomsel { get; set; }
        public double? wow { get; set; }
        public string? summary { get; set; }
    }
}
