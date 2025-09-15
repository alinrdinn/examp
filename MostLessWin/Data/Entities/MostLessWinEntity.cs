using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.MostLessWin.Data.Entities
{
    [Keyless]
    public class MostLessWinEntity
    {
        public int? yearweek { get; set; }
        public string? level { get; set; }
        public string? location { get; set; }
        public int? win { get; set; }
        public string? category { get; set; }
        public string? platform { get; set; } 
        public int? total { get; set; }
    }
}
