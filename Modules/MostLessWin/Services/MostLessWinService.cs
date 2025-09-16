using System.Drawing;
using ClosedXML.Excel;
using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Responses;
using ExecutiveDashboard.Modules.MostLessWin.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace ExecutiveDashboard.Modules.MostLessWin.Services
{
    public class MostLessWinService : IMostLessWinService
    {
        private readonly IMostLessWinRepository _repo;
        private readonly IMemoryCache _cache;

        public MostLessWinService(IMostLessWinRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<MostLessWinResponse> GetMostLessWin(MostLessWinRequest request)
        {
            var cacheKey =
                $"MostLessWin_GetMostLessWin_{request.Yearweek}_{request.Level}_{request.Location}_{request.Source}";

            if (_cache.TryGetValue(cacheKey, out MostLessWinResponse cachedResult))
            {
                return cachedResult;
            }

            var rows = await _repo.GetMostWinForLatestWeek(
                request.Yearweek!.Value,
                request.Level!,
                request.Location!,
                request.Source!
            );

            var mostWin = rows.Where(r => r.category == "win")
                .OrderByDescending(r => r.win ?? int.MinValue)
                .FirstOrDefault();

            var lessWin = rows.Where(r => r.category == "lose")
                .OrderBy(r => r.win ?? int.MaxValue)
                .FirstOrDefault();

            var result = new MostLessWinResponse
            {
                MostWinRegion = mostWin?.location,
                MostWinCount = mostWin?.win,
                MostWinOutOf = mostWin?.total,
                LessWinRegion = lessWin?.location,
                LessWinCount = lessWin?.win,
                LessWinOutOf = lessWin?.total,
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<IXLWorksheet> CreateWinLoseMetricsWorksheet(
            XLWorkbook workbook,
            MostLessWinRequest request,
            string? worksheetName = null
        )
        {
            // Get data from repository
            var data = await _repo.GetMostWinForLatestWeek(
                request.Yearweek!.Value,
                request.Level!,
                request.Location!,
                request.Source!
            );

            var worksheetNameToUse = string.IsNullOrWhiteSpace(worksheetName)
                ? "WinLoseMetrics"
                : worksheetName;

            var worksheet = workbook.Worksheets.Add(worksheetNameToUse);

            // Add metadata section
            worksheet.Cell(1, 1).Value = "Win/Lose Metrics Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Range(1, 1, 1, 2).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add metadata rows
            int currentRow = 2;
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
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Source:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = request.Source;
            currentRow += 2;

            // Add table headers
            worksheet.Cell(currentRow, 1).Value = "Level";
            worksheet.Cell(currentRow, 2).Value = "Location";
            worksheet.Cell(currentRow, 3).Value = "Win";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Platform";
            worksheet.Cell(currentRow, 6).Value = "Total";

            // Style the headers
            var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            // Add data rows
            foreach (var item in data)
            {
                worksheet.Cell(currentRow, 1).Value = item.level;
                worksheet.Cell(currentRow, 2).Value = item.location;
                worksheet.Cell(currentRow, 3).Value = item.win;
                worksheet.Cell(currentRow, 4).Value = item.category;
                worksheet.Cell(currentRow, 5).Value = item.platform;
                worksheet.Cell(currentRow, 6).Value = item.total;

                // Add borders to data rows
                var dataRange = worksheet.Range(currentRow, 1, currentRow, 6);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        public async Task<XLWorkbook> GenerateWinLoseMetricsWorkbook(MostLessWinRequest request)
        {
            var workbook = new XLWorkbook();
            await CreateWinLoseMetricsWorksheet(workbook, request);
            return workbook;
        }

        public async Task<byte[]> GenerateWinLoseMetricsExcelFile(MostLessWinRequest request)
        {
            using var workbook = await GenerateWinLoseMetricsWorkbook(request);
            using var stream = new MemoryStream();

            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
