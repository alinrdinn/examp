namespace ExecutiveDashboard.Modules.SummaryNote.Dtos.Requests
{
    public class SummaryNoteQueryRequest
    {
        public int? Yearweek { get; init; }
    }

    public class CreateSummaryNoteRequest
    {
        public int? Yearweek { get; init; }
        public string? Detail { get; init; }
    }

    public class UpdateSummaryNoteRequest
    {
        public int? Yearweek { get; init; }
        public string? Detail { get; init; }
    }
}
