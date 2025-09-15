using ExecutiveDashboard.Modules.MostLessWin.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<MostLessWinEntity> MostLessWins { get; set; }
    }
}
