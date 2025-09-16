using System.Linq;
using ClosedXML.Excel;
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

        public async Task<XLWorkbook> GenerateSummaryNotesWorkbook(SummaryNoteQueryRequest request)
        {
            var summaryNotes = await GetSummaryNotes(request);

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("SummaryNotes");

            worksheet.Range("A1:B1").Merge().Value = "Metadata";
            worksheet.Range("A1:B1").Style.Font.Bold = true;
            worksheet.Range("A1:B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var currentRow = 2;

            worksheet.Cell(currentRow, 1).Value = "Yearweek";
            worksheet.Cell(currentRow, 2).Value = request.Yearweek?.ToString() ?? "-";
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Notes";
            worksheet.Cell(currentRow, 2).Value = summaryNotes.Total;
            currentRow += 2;

            worksheet.Cell(currentRow, 1).Value = "ID";
            worksheet.Cell(currentRow, 2).Value = "Detail";
            worksheet.Cell(currentRow, 3).Value = "Region";

            var headerRange = worksheet.Range(currentRow, 1, currentRow, 3);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            foreach (var note in summaryNotes.SummarData)
            {
                worksheet.Cell(currentRow, 1).Value = note.Id;
                worksheet.Cell(currentRow, 2).Value = note.Detail;
                worksheet.Cell(currentRow, 3).Value = note.Region;

                var dataRange = worksheet.Range(currentRow, 1, currentRow, 3);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            return workbook;
        }

        public Task CreateSummaryNote(CreateSummaryNoteRequest request)
        {
            return _repository.CreateSummaryNote(request.Yearweek, request.Detail);
        }

        public Task UpdateSummaryNote(int id, UpdateSummaryNoteRequest request)
        {
            return _repository.UpdateSummaryNote(id, request.Yearweek, request.Detail);
        }

        public async Task<byte[]> GenerateSummaryNotesExcelFile(SummaryNoteQueryRequest request)
        {
            using var workbook = await GenerateSummaryNotesWorkbook(request);
            using var stream = new MemoryStream();

            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
