using ExecutiveDashboard.Modules.Summary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<SummaryModel> Summaries { get; set; }
        public DbSet<SummaryGapModel> SummaryGaps { get; set; }
    }
}
