using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<LocationWinLoseEntity> LocationWinLoses { get; set; }
    }
}
