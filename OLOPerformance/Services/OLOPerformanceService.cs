using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Responses;
using ExecutiveDashboard.Modules.OLOPerformance.Repositories;
using ClosedXML.Excel;

namespace ExecutiveDashboard.Modules.OLOPerformance.Services
{
    public class OLOPerformanceService : IOLOPerformanceService
    {
        private readonly IOLOPerformanceRepository _repo;

        public OLOPerformanceService(IOLOPerformanceRepository repo)
        {
            _repo = repo;
        }

        public async Task<OLOPerformanceResponse> GetOloPerformance(OLOPerformanceRequest request)
        {
            var rows = await _repo.GetOloPerformance(request.Yearweek, request.Level, request.Location);

            
            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var operatorTotals = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.@operator))
                .GroupBy(r => r.@operator!)
                .Select(g => new
                {
                    Operator = g.Key,
                    TotalWin = g.Sum(x => x.win ?? 0)
                })
                .OrderByDescending(x => x.TotalWin)
                .ToList();

            var operatorWin = operatorTotals.FirstOrDefault()?.Operator;

            // Helper to read per-platform details for the winning operator
            PlatformWin BuildPlatform(string platformKey)
            {
                var row = rows.FirstOrDefault(r =>
                    string.Equals(r.@operator, operatorWin, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.platform, platformKey, StringComparison.OrdinalIgnoreCase));

                var score = row?.win ?? 0;
                var wowPct = $"{Math.Round(((row?.wow) ?? 0d) * 100d, 0)}%";
                var status = string.Equals(row?.remark, "UP", StringComparison.OrdinalIgnoreCase);

                return new PlatformWin
                {
                    Score = score,
                    Wow = wowPct,
                    Status = status
                };
            }

            // Build "otherOperator" list (excluding the winner)
            var others = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.@operator) &&
                            !string.Equals(r.@operator, operatorWin, StringComparison.OrdinalIgnoreCase))
                .GroupBy(r => r.@operator!)
                .Select(g =>
                {
                    var os = g.Where(x => string.Equals(x.platform, "os", StringComparison.OrdinalIgnoreCase))
                              .Sum(x => x.win ?? 0);
                    var ookla = g.Where(x => string.Equals(x.platform, "ookla", StringComparison.OrdinalIgnoreCase))
                                 .Sum(x => x.win ?? 0);
                    return new OtherOperatorItem
                    {
                        Operator = g.Key,
                        Os = os,
                        Ookla = ookla
                    };
                })
                .OrderByDescending(x => (x.Os ?? 0) + (x.Ookla ?? 0))
                .ToList();

            return new OLOPerformanceResponse
            {
                OperatorWin = operatorWin,
                SourceWin = new SourceWin
                {
                    OpenSignalWin = BuildPlatform("os"),
                    OoklaWin = BuildPlatform("ookla")
                },
                OtherOperator = others
            };
        }

        public async Task<XLWorkbook> GenerateOloPerformanceWorkbook(OLOPerformanceRequest request)
        {
            var rows = await _repo.GetOloPerformance(request.Yearweek, request.Level, request.Location);

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("OLO Performance");

            // Add Metadata Section
            var currentRow = 1;
            
            // Metadata header
            worksheet.Range($"A{currentRow}:B{currentRow}").Merge().Value = "Metadata";
            worksheet.Range($"A{currentRow}:B{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:B{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
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

            return workbook;
        }

        public async Task<byte[]> GenerateOloPerformanceExcelFile(OLOPerformanceRequest request)
        {
            using var workbook = await GenerateOloPerformanceWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
