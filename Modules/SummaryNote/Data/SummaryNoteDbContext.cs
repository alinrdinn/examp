using ExecutiveDashboard.Modules.SummaryNote.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Common
{
    public partial class ExecutiveDbContext : DbContext
    {
        public DbSet<SummaryNoteEntity> SummaryNotes { get; set; }
    }
}
