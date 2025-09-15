using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.Filter.Data.Entities
{
    [Keyless]
    public class WeekReference
    {
        public int? yearweek { get; set; }
    }
}
