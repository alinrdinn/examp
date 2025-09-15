using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities
{
    [Keyless]
    public class LocationWinLoseEntity
    {
        public int? yearweek { get; set; }
        public string? location { get; set; }
        public int? win { get; set; }
        public int? lose { get; set; }
        public double? percentage { get; set; }
    }
}
