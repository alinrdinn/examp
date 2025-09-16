using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public ExecutiveDbContext(DbContextOptions<ExecutiveDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
