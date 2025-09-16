using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Requests;
using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Responses;
using ExecutiveDashboard.Modules.WinLoseMetric.Repositories;
using ClosedXML.Excel;
using Microsoft.Extensions.Caching.Memory;

namespace ExecutiveDashboard.Modules.WinLoseMetric.Services
{
    public class WinLoseMetricService : IWinLoseMetricService
    {
        private readonly IWinLoseMetricRepository _repo;
        private readonly IMemoryCache _cache;

        public WinLoseMetricService(IWinLoseMetricRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<WinLoseMetricResponse> GetWinLoseMetrics(WinLoseMetricRequest request)
        {
            var cacheKey = $"WinLoseMetric_GetWinLoseMetrics_{request.Yearweek}_{request.Level}_{request.Location}_{request.Source}_{request.Status}";

            if (_cache.TryGetValue(cacheKey, out WinLoseMetricResponse cachedResult))
            {
                return cachedResult;
            }

            var rows = await _repo.GetWinLoseMetrics(
                request.Yearweek,
                request.Level,
                request.Location,
                request.Source,
                request.Status
            );

            if (!rows.Any())
            {
                throw new NotFoundException("not_found");
            }

            var first = rows.FirstOrDefault();

            var response = new WinLoseMetricResponse
            {
                Percentage = first?.percentage,
                Total = first?.win,
                Increase = first?.increase,
                Decrease = first?.decrease,
                Metrics = rows.Select(r => new MetricItem
                    {
                        Title = r.metric,
                        Value = r.score,
                        Status = string.Equals(r.remark, "UP", StringComparison.OrdinalIgnoreCase),
                        Wow = r.wow,
                    })
                    .ToList(),
            };

            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            return response;
        }

        public async Task<IXLWorksheet> CreateWinLoseMetricsWorksheet(
            XLWorkbook workbook,
            WinLoseMetricRequest request,
            string? worksheetName = null
        )
        {
            // Get data using existing method
            var response = await GetWinLoseMetrics(request);

            var worksheetNameToUse = string.IsNullOrWhiteSpace(worksheetName)
                ? "WinLoseMetrics"
                : worksheetName;

            var worksheet = workbook.Worksheets.Add(worksheetNameToUse);

            int currentRow = 1;

            // Section 1: Metadata
            worksheet.Range("A1:B1").Merge().Value = "Metadata";
            worksheet.Range("A1:B1").Style.Font.Bold = true;
            worksheet.Range("A1:B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow = 2;
            
            worksheet.Cell(currentRow, 1).Value = "Yearweek";
            worksheet.Cell(currentRow, 2).Value = request.Yearweek;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Level";
            worksheet.Cell(currentRow, 2).Value = request.Level;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Location";
            worksheet.Cell(currentRow, 2).Value = request.Location;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Source";
            worksheet.Cell(currentRow, 2).Value = request.Source;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Status";
            worksheet.Cell(currentRow, 2).Value = request.Status;
            currentRow += 2; // Add empty row
            
            // Section 2: Summary Statistics
            worksheet.Cell(currentRow, 1).Value = "Total Win";
            worksheet.Cell(currentRow, 2).Value = response.Total;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Increase";
            worksheet.Cell(currentRow, 2).Value = response.Increase;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Decrease";
            worksheet.Cell(currentRow, 2).Value = response.Decrease;
            currentRow++;
            
            worksheet.Cell(currentRow, 1).Value = "Percentage";
            worksheet.Cell(currentRow, 2).Value = $"{response.Percentage}%";
            currentRow += 2; // Add empty row
            
            // Section 3: Metrics Table
            // Headers
            worksheet.Cell(currentRow, 1).Value = "Title";
            worksheet.Cell(currentRow, 2).Value = "Value";
            worksheet.Cell(currentRow, 3).Value = "Status";
            worksheet.Cell(currentRow, 4).Value = "Wow";
            
            // Style headers
            var headerRange = worksheet.Range(currentRow, 1, currentRow, 4);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;
            
            // Data rows
            foreach (var metric in response.Metrics)
            {
                worksheet.Cell(currentRow, 1).Value = metric.Title;
                worksheet.Cell(currentRow, 2).Value = metric.Value;
                worksheet.Cell(currentRow, 3).Value = metric.Status;
                worksheet.Cell(currentRow, 4).Value = metric.Wow;
                currentRow++;
            }
            
            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        public async Task<XLWorkbook> GenerateWinLoseMetricsWorkbook(WinLoseMetricRequest request)
        {
            var workbook = new XLWorkbook();
            await CreateWinLoseMetricsWorksheet(workbook, request);
            return workbook;
        }

        public async Task<byte[]> GenerateWinLoseMetricsExcelFile(WinLoseMetricRequest request)
        {
            using var workbook = await GenerateWinLoseMetricsWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
