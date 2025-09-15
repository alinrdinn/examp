using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.Summary.Data.Entities;
using ExecutiveDashboard.Modules.Summary.Dtos.Requests;
using ExecutiveDashboard.Modules.Summary.Dtos.Responses;
using ExecutiveDashboard.Modules.Summary.Repositories;

namespace ExecutiveDashboard.Modules.Summary.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository _repo;

        public SummaryService(ISummaryRepository repo)
        {
            _repo = repo;
        }

        private GapMetric MapGapMetric(List<SummaryGapModel> rows, string category)
        {
            return new GapMetric
            {
                Category = category,
                Strong =
                    rows.Where(r =>
                            r.remark?.ToUpper() == "INCREASE"
                            || r.remark?.ToLower() == "strong_position"
                        )
                        .Select(r => new MetricDetail { Label = r.metric, Value = r.percent })
                        .FirstOrDefault() ?? new MetricDetail(),
                Weak =
                    rows.Where(r =>
                            r.remark?.ToUpper() == "DECREASE"
                            || r.remark?.ToLower() == "week_position"
                        )
                        .Select(r => new MetricDetail { Label = r.metric, Value = r.percent })
                        .FirstOrDefault() ?? new MetricDetail(),
            };
        }

        public async Task<SummaryResponse> GetSummary(SummaryRequest request)
        {
            // Total Win
            var rows = await _repo.GetSummary(request.Yearweek, request.Level, request.Location);

            if (!rows.Any())
            {
                return new SummaryResponse();
            }

            var totalTelkomsel = rows.FirstOrDefault(r => r.platform == "all");
            var os = rows.FirstOrDefault(r => r.platform == "os");
            var ookla = rows.FirstOrDefault(r => r.platform == "ookla");

            var response = new SummaryResponse
            {
                TotalTelkomselWin = totalTelkomsel?.win,
                PercentageTelkomselWin = totalTelkomsel?.percent,
                TotalOSWin = os?.win,
                PercentageOSWin = os?.percent,
                TotalOoklaWin = ookla?.win,
                PercentageOoklaWin = ookla?.percent,
                TotalTelkomselLose = totalTelkomsel?.lose,
                TotalOSLose = os?.lose,
                TotalOoklaLose = ookla?.lose,
            };

            // Gap To OLO
            try
            {
                var gapToOloRowsOs = await _repo.GetGapToOLO(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "os"
                );
                var gapToOloRowsOokla = await _repo.GetGapToOLO(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "ookla"
                );

                response.GapToOLO = new List<GapMetric>();
                if (gapToOloRowsOs.Any())
                {
                    response.GapToOLO.Add(MapGapMetric(gapToOloRowsOs, "open_signal"));
                }
                if (gapToOloRowsOokla.Any())
                {
                    response.GapToOLO.Add(MapGapMetric(gapToOloRowsOokla, "ookla"));
                }
            }
            catch
            {
                response.GapToOLO = new List<GapMetric>();
            }

            // Wow From Week
            try
            {
                var wowFromWeekOs = await _repo.GetWowFromWeek(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "os"
                );
                var wowFromWeekOokla = await _repo.GetWowFromWeek(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "ookla"
                );

                response.WowFromWeek = new List<GapMetric>();
                if (wowFromWeekOs.Any())
                {
                    response.WowFromWeek.Add(MapGapMetric(wowFromWeekOs, "open_signal"));
                }
                if (wowFromWeekOokla.Any())
                {
                    response.WowFromWeek.Add(MapGapMetric(wowFromWeekOokla, "ookla"));
                }
            }
            catch
            {
                response.WowFromWeek = new List<GapMetric>();
            }

            // Gap To H1
            try
            {
                var gapToH1Os = await _repo.GetGapToH1(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "os"
                );
                var gapToH1Ookla = await _repo.GetGapToH1(
                    request.Yearweek,
                    request.Level,
                    request.Location,
                    "ookla"
                );

                response.GapToH1 = new List<GapMetric>();
                if (gapToH1Os.Any())
                {
                    response.GapToH1.Add(MapGapMetric(gapToH1Os, "open_signal"));
                }
                if (gapToH1Ookla.Any())
                {
                    response.GapToH1.Add(MapGapMetric(gapToH1Ookla, "ookla"));
                }
            }
            catch
            {
                response.GapToH1 = new List<GapMetric>();
            }

            return response;
        }

        public async Task<XLWorkbook> GenerateSummaryWorkbook(SummaryRequest request)
        {
            // Ambil data dari service yang sudah ada
            var summary = await GetSummary(request);

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Summary");

            // === Metadata ===
            worksheet.Range("A1:B1").Merge().Value = "Metadata";
            worksheet.Range("A1:B1").Style.Font.Bold = true;
            worksheet.Range("A1:B1").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            int currentRow = 3;
            worksheet.Cell(currentRow, 1).Value = "Yearweek:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = request.Yearweek;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Level:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = request.Level;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Location:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = request.Location;
            currentRow += 2;

            // === Section: Total Wins & Loses ===
            worksheet.Cell(currentRow, 1).Value = "Summary Totals";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Telkomsel Win";
            worksheet.Cell(currentRow, 2).Value = summary.TotalTelkomselWin;
            worksheet.Cell(currentRow, 3).Value = $"{summary.PercentageTelkomselWin}%";
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total OS Win";
            worksheet.Cell(currentRow, 2).Value = summary.TotalOSWin;
            worksheet.Cell(currentRow, 3).Value = $"{summary.PercentageOSWin}%";
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Ookla Win";
            worksheet.Cell(currentRow, 2).Value = summary.TotalOoklaWin;
            worksheet.Cell(currentRow, 3).Value = $"{summary.PercentageOoklaWin}%";
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Telkomsel Lose";
            worksheet.Cell(currentRow, 2).Value = summary.TotalTelkomselLose;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total OS Lose";
            worksheet.Cell(currentRow, 2).Value = summary.TotalOSLose;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Ookla Lose";
            worksheet.Cell(currentRow, 2).Value = summary.TotalOoklaLose;
            currentRow += 2;

            // === Section: GapToOLO, WowFromWeek, GapToH1 ===
            void AddGapSection(string title, List<GapMetric>? metrics)
            {
                worksheet.Cell(currentRow, 1).Value = title;
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                currentRow++;

                // header
                worksheet.Cell(currentRow, 1).Value = "Category";
                worksheet.Cell(currentRow, 2).Value = "Strong Label";
                worksheet.Cell(currentRow, 3).Value = "Strong Value";
                worksheet.Cell(currentRow, 4).Value = "Weak Label";
                worksheet.Cell(currentRow, 5).Value = "Weak Value";

                var headerRange = worksheet.Range(currentRow, 1, currentRow, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;

                if (metrics != null)
                {
                    foreach (var m in metrics)
                    {
                        worksheet.Cell(currentRow, 1).Value = m.Category;
                        worksheet.Cell(currentRow, 2).Value = m.Strong?.Label;
                        worksheet.Cell(currentRow, 3).Value = m.Strong?.Value;
                        worksheet.Cell(currentRow, 4).Value = m.Weak?.Label;
                        worksheet.Cell(currentRow, 5).Value = m.Weak?.Value;

                        var dataRange = worksheet.Range(currentRow, 1, currentRow, 5);
                        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        currentRow++;
                    }
                }

                currentRow += 2;
            }

            AddGapSection("Gap To OLO", summary.GapToOLO);
            AddGapSection("Wow From Week", summary.WowFromWeek);
            AddGapSection("Gap To H1", summary.GapToH1);

            worksheet.Columns().AdjustToContents();

            return workbook;
        }

        public async Task<byte[]> GenerateSummaryExcelFile(SummaryRequest request)
        {
            using var workbook = await GenerateSummaryWorkbook(request);
            using var stream = new MemoryStream();

            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
