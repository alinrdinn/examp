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

    [Keyless]
    public class ImproveMaintainDegradeEntity
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? area { get; set; }
        public string? region { get; set; }
        public string? location { get; set; }
        public string? remark { get; set; }
        public string? summary { get; set; }
        public string? status { get; set; }
    }
    
    
    [Keyless]
    public class AreaStatusEntity
    {
        public string? area { get; set; }
        public string? remark_area { get; set; } 
    }
}
