using ExecutiveDashboard.Modules.OLOPerformance.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<OLOPerformanceEntity> OLOPerformances { get; set; }
    }
}
