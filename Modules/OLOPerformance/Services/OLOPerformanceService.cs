using ClosedXML.Excel;
using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses;
using ExecutiveDashboard.Modules.OLOPerformance.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace ExecutiveDashboard.Modules.OLOPerformance.Services
{
    public class OLOPerformanceService : IOLOPerformanceService
    {
        private readonly IOLOPerformanceRepository _repo;
        private readonly IMemoryCache _cache;

        public OLOPerformanceService(IOLOPerformanceRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<OLOPerformanceResponse> GetOloPerformance(OLOPerformanceRequest request)
        {
            var cacheKey =
                $"OLOPerformance_GetOloPerformance_{request.Yearweek}_{request.Level}_{request.Location}";

            if (_cache.TryGetValue(cacheKey, out OLOPerformanceResponse cachedResult))
            {
                return cachedResult;
            }

            var rows = await _repo.GetOloPerformance(
                request.Yearweek,
                request.Level,
                request.Location
            );

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var operatorTotals = rows.Where(r => !string.IsNullOrWhiteSpace(r.@operator))
                .GroupBy(r => r.@operator!)
                .Select(g => new { Operator = g.Key, TotalWin = g.Sum(x => x.win ?? 0) })
                .OrderByDescending(x => x.TotalWin)
                .ToList();

            var operatorWin = operatorTotals.FirstOrDefault()?.Operator;

            // Helper to read per-platform details for the winning operator
            PlatformWin BuildPlatform(string platformKey)
            {
                var row = rows.FirstOrDefault(r =>
                    string.Equals(r.@operator, operatorWin, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(r.platform, platformKey, StringComparison.OrdinalIgnoreCase)
                );

                var score = row?.win ?? 0;
                var wowPct = $"{Math.Round(((row?.wow) ?? 0d), 0)}%";
                var status = row?.remark;

                return new PlatformWin
                {
                    Score = score,
                    Wow = wowPct,
                    Status = status,
                };
            }

            // Build "otherOperator" list (excluding the winner)
            var others = rows.Where(r =>
                    !string.IsNullOrWhiteSpace(r.@operator)
                    && !string.Equals(r.@operator, operatorWin, StringComparison.OrdinalIgnoreCase)
                )
                .GroupBy(r => r.@operator!)
                .Select(g =>
                {
                    var os = g.Where(x =>
                            string.Equals(x.platform, "os", StringComparison.OrdinalIgnoreCase)
                        )
                        .Sum(x => x.win ?? 0);
                    var ookla = g.Where(x =>
                            string.Equals(x.platform, "ookla", StringComparison.OrdinalIgnoreCase)
                        )
                        .Sum(x => x.win ?? 0);
                    return new OtherOperatorItem
                    {
                        Operator = g.Key,
                        Os = os,
                        Ookla = ookla,
                    };
                })
                .OrderByDescending(x => (x.Os ?? 0) + (x.Ookla ?? 0))
                .ToList();

            var result = new OLOPerformanceResponse
            {
                OperatorWin = operatorWin,
                SourceWin = new SourceWin
                {
                    OpenSignalWin = BuildPlatform("os"),
                    OoklaWin = BuildPlatform("ookla"),
                },
                OtherOperator = others,
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<OLOPerformanceSummaryResponse> GetOloPerformanceSummary(
            OLOPerformanceRequest request
        )
        {
            var rows = await _repo.GetOloPerformanceSummary(
                request.Yearweek,
                request.Level,
                request.Location
            );

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var first = rows.First();

            var response = new OLOPerformanceSummaryResponse
            {
                Operator = first.@operator,
                OpenSignal = rows.Where(r =>
                        r.platform?.ToLower() == "opensignal" || r.platform?.ToLower() == "os"
                    )
                    .Select(r => new OLOPerformanceDetail
                    {
                        Title = r.metric,
                        Score = r.value,
                        Improvement = r.wow.HasValue ? $"{r.wow:0.##}%" : null,
                        GapWithTelkomsel = r.gap_telkomsel.HasValue
                            ? $"{r.gap_telkomsel:0.##}%"
                            : null,
                    })
                    .FirstOrDefault(),
                Ookla = rows.Where(r => r.platform?.ToLower() == "ookla")
                    .Select(r => new OLOPerformanceDetail
                    {
                        Title = r.metric,
                        Score = r.value,
                        Improvement = r.wow.HasValue ? $"{r.wow:0.##}%" : null,
                        GapWithTelkomsel = r.gap_telkomsel.HasValue
                            ? $"{r.gap_telkomsel:0.##}%"
                            : null,
                    })
                    .FirstOrDefault(),
                Summary = rows.Select(r => r.summary ?? string.Empty).ToList(),
            };

            return response;
        }

        public async Task<IXLWorksheet> CreateOloPerformanceWorksheet(
            XLWorkbook workbook,
            OLOPerformanceRequest request,
            string? worksheetName = null
        )
        {
            var rows = await _repo.GetOloPerformance(
                request.Yearweek,
                request.Level,
                request.Location
            );

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var worksheetNameToUse = string.IsNullOrWhiteSpace(worksheetName)
                ? "OLO Performance"
                : worksheetName;

            var worksheet = workbook.Worksheets.Add(worksheetNameToUse);

            // Add Metadata Section
            var currentRow = 1;

            // Metadata header
            worksheet.Range($"A{currentRow}:B{currentRow}").Merge().Value = "Metadata";
            worksheet.Range($"A{currentRow}:B{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:B{currentRow}").Style.Fill.BackgroundColor =
                XLColor.LightGray;
            currentRow++;

            // Metadata content
            worksheet.Cell(currentRow, 1).Value = "Yearweek";
            worksheet.Cell(currentRow, 2).Value = request.Yearweek;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Level";
            worksheet.Cell(currentRow, 2).Value = request.Level;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Location";
            worksheet.Cell(currentRow, 2).Value = request.Location;
            currentRow++;

            // Headers
            worksheet.Cell(currentRow, 1).Value = "Operator";
            worksheet.Cell(currentRow, 2).Value = "Platform";
            worksheet.Cell(currentRow, 3).Value = "Win";
            worksheet.Cell(currentRow, 4).Value = "WoW";
            worksheet.Cell(currentRow, 5).Value = "Remark";

            // Style headers
            var headerRange = worksheet.Range(currentRow, 1, currentRow, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            // Data rows
            var filteredRows = rows.Where(r => !string.IsNullOrWhiteSpace(r.@operator)).ToList();
            foreach (var row in filteredRows)
            {
                worksheet.Cell(currentRow, 1).Value = row.@operator;
                worksheet.Cell(currentRow, 2).Value = row.platform;
                worksheet.Cell(currentRow, 3).Value = row.win;
                worksheet.Cell(currentRow, 4).Value = $"{Math.Round(((row.wow) ?? 0d) * 100d, 1)}%";
                worksheet.Cell(currentRow, 5).Value = row.remark;
                currentRow++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        public async Task<XLWorkbook> GenerateOloPerformanceWorkbook(OLOPerformanceRequest request)
        {
            var workbook = new XLWorkbook();
            await CreateOloPerformanceWorksheet(workbook, request);
            return workbook;
        }

        public async Task<byte[]> GenerateOloPerformanceExcelFile(OLOPerformanceRequest request)
        {
            using var workbook = await GenerateOloPerformanceWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<IXLWorksheet> CreateOloPerformanceSummaryWorksheet(
            XLWorkbook workbook,
            OLOPerformanceRequest request,
            string? worksheetName = null
        )
        {
            var summary = await GetOloPerformanceSummary(request);

            var worksheetNameToUse = string.IsNullOrWhiteSpace(worksheetName)
                ? "OLO Performance Summary"
                : worksheetName;

            var worksheet = workbook.Worksheets.Add(worksheetNameToUse);

            int currentRow = 1;

            // === Metadata Section ===
            worksheet.Range("A1:B1").Merge().Value = "Metadata";
            worksheet.Range("A1:B1").Style.Font.Bold = true;
            worksheet.Range("A1:B1").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            currentRow = 3;
            worksheet.Cell(currentRow, 1).Value = "Yearweek";
            worksheet.Cell(currentRow, 2).Value = request.Yearweek;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Level";
            worksheet.Cell(currentRow, 2).Value = request.Level;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Location";
            worksheet.Cell(currentRow, 2).Value = request.Location;
            currentRow += 2;

            // === Operator Info ===
            worksheet.Cell(currentRow, 1).Value = "Operator";
            worksheet.Cell(currentRow, 2).Value = summary.Operator;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow += 2;

            // === OpenSignal Section ===
            worksheet.Cell(currentRow, 1).Value = "OpenSignal";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Title";
            worksheet.Cell(currentRow, 2).Value = summary.OpenSignal?.Title;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Score";
            worksheet.Cell(currentRow, 2).Value = summary.OpenSignal?.Score;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Improvement";
            worksheet.Cell(currentRow, 2).Value = summary.OpenSignal?.Improvement;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Gap With Telkomsel";
            worksheet.Cell(currentRow, 2).Value = summary.OpenSignal?.GapWithTelkomsel;
            currentRow += 2;

            // === Ookla Section ===
            worksheet.Cell(currentRow, 1).Value = "Ookla";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Title";
            worksheet.Cell(currentRow, 2).Value = summary.Ookla?.Title;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Score";
            worksheet.Cell(currentRow, 2).Value = summary.Ookla?.Score;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Improvement";
            worksheet.Cell(currentRow, 2).Value = summary.Ookla?.Improvement;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Gap With Telkomsel";
            worksheet.Cell(currentRow, 2).Value = summary.Ookla?.GapWithTelkomsel;
            currentRow += 2;

            // === Summary Section ===
            worksheet.Cell(currentRow, 1).Value = "Summary";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            if (summary.Summary != null && summary.Summary.Any())
            {
                foreach (var line in summary.Summary)
                {
                    worksheet.Cell(currentRow, 1).Value = line;
                    currentRow++;
                }
            }

            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        public async Task<XLWorkbook> GenerateOloPerformanceSummaryWorkbook(
            OLOPerformanceRequest request
        )
        {
            var workbook = new XLWorkbook();
            await CreateOloPerformanceSummaryWorksheet(workbook, request);
            return workbook;
        }

        public async Task<byte[]> GenerateOloPerformanceSummaryExcelFile(
            OLOPerformanceRequest request
        )
        {
            using var workbook = await GenerateOloPerformanceSummaryWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
