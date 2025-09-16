using System.Collections.Generic;

namespace ExecutiveDashboard.Modules.SummaryNote.Dtos.Responses
{
    public class SummaryNoteItemResponse
    {
        public int Id { get; init; }
        public string? Detail { get; init; }
        public string? Region { get; init; }
    }

    public class SummaryNoteListResponse
    {
        public int Total { get; init; }
        public IReadOnlyCollection<SummaryNoteItemResponse> SummarData { get; init; } =
            new List<SummaryNoteItemResponse>();
    }
}
