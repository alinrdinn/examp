using ExecutiveDashboard.Modules.SummaryNote.Dtos.Requests;
using ExecutiveDashboard.Modules.SummaryNote.Dtos.Responses;

namespace ExecutiveDashboard.Modules.SummaryNote.Services
{
    public interface ISummaryNoteService
    {
        Task<SummaryNoteListResponse> GetSummaryNotes(SummaryNoteQueryRequest request);
        Task CreateSummaryNote(CreateSummaryNoteRequest request);
        Task UpdateSummaryNote(int id, UpdateSummaryNoteRequest request);
        Task<byte[]> GenerateSummaryNotesExcelFile(SummaryNoteQueryRequest request);
    }
}
