using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<LocationWinLoseEntity> LocationWinLoses { get; set; }
        public DbSet<ImproveMaintainDegradeEntity> ImproveMaintainDegradeEntities { get; set; }
        public DbSet<AreaStatusEntity> AreaStatusEntities { get; set; }
    }
}
