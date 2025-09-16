using Microsoft.EntityFrameworkCore;

namespace ExecutiveDashboard.Modules.SummaryNote.Data.Entities
{
    [Keyless]
    public class SummaryNoteEntity
    {
        public int? yearweek { get; set; }
        public int id { get; set; }
        public string? summary { get; set; }
    }
}
