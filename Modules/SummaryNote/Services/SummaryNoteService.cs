using System.Linq;
using ExecutiveDashboard.Modules.SummaryNote.Dtos.Requests;
using ExecutiveDashboard.Modules.SummaryNote.Dtos.Responses;
using ExecutiveDashboard.Modules.SummaryNote.Repositories;

namespace ExecutiveDashboard.Modules.SummaryNote.Services
{
    public class SummaryNoteService : ISummaryNoteService
    {
        private readonly ISummaryNoteRepository _repository;

        public SummaryNoteService(ISummaryNoteRepository repository)
        {
            _repository = repository;
        }

        public async Task<SummaryNoteListResponse> GetSummaryNotes(SummaryNoteQueryRequest request)
        {
            var rows = await _repository.GetSummaryNotes(request.Yearweek);

            var items = rows
                .Select(row => new SummaryNoteItemResponse
                {
                    Id = row.id,
                    Detail = row.summary?.Trim(),
                    Region = string.Empty,
                })
                .ToList();

            return new SummaryNoteListResponse
            {
                Total = items.Count,
                SummarData = items,
            };
        }

        public Task CreateSummaryNote(CreateSummaryNoteRequest request)
        {
            return _repository.CreateSummaryNote(request.Yearweek, request.Detail);
        }

        public Task UpdateSummaryNote(int id, UpdateSummaryNoteRequest request)
        {
            return _repository.UpdateSummaryNote(id, request.Yearweek, request.Detail);
        }
    }
}
