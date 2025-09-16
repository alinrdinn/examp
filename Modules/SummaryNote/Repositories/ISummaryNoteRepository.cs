using ExecutiveDashboard.Modules.SummaryNote.Data.Entities;

namespace ExecutiveDashboard.Modules.SummaryNote.Repositories
{
    public interface ISummaryNoteRepository
    {
        Task<List<SummaryNoteEntity>> GetSummaryNotes(int? yearweek);
        Task CreateSummaryNote(int? yearweek, string? detail);
        Task UpdateSummaryNote(int id, int? yearweek, string? detail);
    }
}
