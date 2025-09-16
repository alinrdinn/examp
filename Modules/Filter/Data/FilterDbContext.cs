using ExecutiveDashboard.Modules.Filter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<CityReference> CityReferences { get; set; }
        public DbSet<RegionReference> RegionReferences { get; set; }
        public DbSet<WeekReference> WeekReferences { get; set; }
    }
}
