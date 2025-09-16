using ExecutiveDashboard.Modules.WinLoseMetric.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<WinLoseMetricEntity> WinLoseMetrics { get; set; }
    }
}
