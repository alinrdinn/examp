using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Responses;
using ExecutiveDashboard.Modules.ImproveDegrade.Repositories;
using ClosedXML.Excel;
using System.Drawing;
using Microsoft.Extensions.Caching.Memory;
using ExecutiveDashboard.Modules.ImproveDegrade.Data.Entities;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Services
{
    public class ImproveDegradeService : IImproveDegradeService
    {
        private readonly IImproveDegradeRepository _repo;
        private readonly IMemoryCache _cache;

        public ImproveDegradeService(IImproveDegradeRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }
        
        public async Task<List<NewImproveDegradeResponse>> GetImproveDegrade(ImproveDegradeRequest request)
        {
            // var cacheKey = $"ImproveDegrade_GetImproveDegrade_{request.Yearweek}_{request.Source}";
            // if (_cache.TryGetValue(cacheKey, out List<ImproveDegradeResponse> cached)) return cached;

            var rows = await _repo.GetAreaWinLose(request.Yearweek!.Value, request.Source!);

            var mapping = await _repo.GetRegionCityMappings(request.Yearweek!.Value, request.Source!);

            var areaStatus = await _repo.GetAreaStatusMappings(request.Yearweek!.Value, request.Source);


            var data = new List<ImproveDegradeResponse>();
            foreach (var r in rows)
            {
                var areaMapping = mapping.Where(x => x.area == r.location).ToList();
                var improveGroup = BuildGroup(areaMapping, "IMPROVE");
                var maintainGroup = BuildGroup(areaMapping, "MAINTAIN");
                var degradeGroup = BuildGroup(areaMapping, "DEGRADE");
                data.Add(new ImproveDegradeResponse
                {
                    Location = r.location,
                    Score = r.win ?? 0,
                    Lose = r.lose ?? 0,
                    Percentage = r.percentage,
                    Improve = improveGroup,
                    Degrade = degradeGroup,
                    Maintain = maintainGroup
                });
            }

            var newData = new List<NewImproveDegradeResponse>();
            
            List<string> orderList = new List<string>
            {
                "Area 1",
                "Sumbagut",
                "Sumbagteng",
                "Sumbagsel",
                "Area 2",
                "Jakarta Banten",
                "Eastern Jabotabek",
                "Jabar",
                "Area 3",
                "Jateng-DIY",
                "Jatim",
                "Bali Nusra",
                "Area 4",
                "Kalimantan",
                "Sulawesi",
                "Maluku dan Papua"
            };

            foreach (var item in data)
            {
                var combinedRegions = new List<NewRegionItem>();
                combinedRegions.AddRange(item.Improve.Regions.Select(x => new NewRegionItem
                {
                    Name = x.Name,
                    Details = x.Details,
                    Status = x.Status,
                    Remark = "IMPROVE"
                }));

                combinedRegions.AddRange(item.Degrade.Regions.Select(x => new NewRegionItem
                {
                    Name = x.Name,
                    Details = x.Details,
                    Status = x.Status,
                    Remark = "DEGRADE"
                }));

                
                combinedRegions.AddRange(item.Maintain.Regions.Select(x => new NewRegionItem
                {
                    Name = x.Name,
                    Details = x.Details,
                    Status = x.Status,
                    Remark = "MAINTAIN"
                }));



                var orderMap = orderList
                    .Select((value, index) => new { value = value.ToLower(), index })
                    .ToDictionary(x => x.value, x => x.index);

                var ordered = combinedRegions
                    .OrderBy(obj =>
                    {
                        string key = obj.Name?.ToLower(); // null-safe
                        return orderMap.TryGetValue(key, out int index) ? index : int.MaxValue;
                    })
                    .ToList();


                newData.Add(new NewImproveDegradeResponse
                {
                    Location = item.Location,
                    Score = item.Score,
                    Lose = item.Lose,
                    Percentage = item.Percentage,
                    Status = areaStatus.FirstOrDefault(x => x.area == item.Location)?.remark_area,
                    Regions = ordered,
                    
                    Improve = item.Improve,
                    Degrade = item.Degrade,
                    Maintain = item.Maintain
                });
            }

            // _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
            return newData;
        }

        private static ImproveGroup BuildGroup(List<ImproveMaintainDegradeEntity> rows, string targetRemark)
        {
            var regions = rows
                .Where(r => string.Equals(r.remark, targetRemark, StringComparison.OrdinalIgnoreCase))
                .Select(r => r.region)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var items = new List<RegionItem>();
            foreach (var region in regions)
            {
                var regionRows = rows.Where(r => string.Equals(r.region, region, StringComparison.OrdinalIgnoreCase));

                var details = regionRows
                    .Select(rr => rr.summary?.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                var statusJoined = string.Join("; ",
                    regionRows
                        .Select(rr => rr.status?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => TrimTrailingPunctuation(s!))
                        .Distinct(StringComparer.Ordinal));

                items.Add(new RegionItem
                {
                    Name = region,
                    Details = details.Count > 0 ? details : null,
                    Status = string.IsNullOrWhiteSpace(statusJoined) ? null : statusJoined
                });
            }

            return new ImproveGroup
            {
                Total = items.Count,
                Regions = items
            };
        }

        private static string TrimTrailingPunctuation(string s)
        {
            return s.Trim().TrimEnd('.', ',').Trim();
        }

        public async Task<IXLWorksheet> CreateWinLoseWorksheet(
            XLWorkbook workbook,
            ImproveDegradeRequest request,
            string? worksheetName = null
        )
        {
            // Get data from existing service method
            var data = await GetImproveDegrade(request);

            var worksheetNameToUse = string.IsNullOrWhiteSpace(worksheetName)
                ? "ImproveDegrade Report"
                : worksheetName;

            var worksheet = workbook.Worksheets.Add(worksheetNameToUse);

            int currentRow = 1;

            // Section 1: Metadata
            currentRow = AddMetadataSection(worksheet, request, currentRow);
            currentRow += 2; // Add spacing

            // Section 2: Summary Statistics Table
            currentRow = AddSummaryStatisticsSection(worksheet, data, currentRow);
            currentRow += 2; // Add spacing

            // Section 3: Improved Detail Table
            currentRow = AddImprovedDetailSection(worksheet, data, currentRow);
            currentRow += 2; // Add spacing

            // Section 4: Degrade Detail Table
            currentRow = AddDegradeDetailSection(worksheet, data, currentRow);

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        public async Task<XLWorkbook> GenerateWinLoseWorkbook(ImproveDegradeRequest request)
        {
            var workbook = new XLWorkbook();
            await CreateWinLoseWorksheet(workbook, request);
            return workbook;
        }

        public async Task<byte[]> GenerateWinLoseExcelFile(ImproveDegradeRequest request)
        {
            using var workbook = await GenerateWinLoseWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private int AddMetadataSection(IXLWorksheet worksheet, ImproveDegradeRequest request, int startRow)
        {
            // Header
            var headerRange = worksheet.Range(startRow, 1, startRow, 2);
            headerRange.Merge().Value = "Metadata";
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = startRow + 1;
            
            // Metadata rows
            worksheet.Cell(row, 1).Value = "Yearweek";
            worksheet.Cell(row, 2).Value = request.Yearweek;
            row++;
            
            worksheet.Cell(row, 1).Value = "Source";
            var sourceValue = request.Source == "open_signal" ? "os" : request.Source;
            worksheet.Cell(row, 2).Value = sourceValue;
            row++;
            
            return row;
        }

        private int AddSummaryStatisticsSection(IXLWorksheet worksheet, List<NewImproveDegradeResponse> data, int startRow)
        {
            int row = startRow;
            
            // Headers
            worksheet.Cell(row, 1).Value = "Location";
            worksheet.Cell(row, 2).Value = "Score";
            worksheet.Cell(row, 3).Value = "Lose";
            worksheet.Cell(row, 4).Value = "Percentage";
            worksheet.Cell(row, 5).Value = "Region Improved Total";
            worksheet.Cell(row, 6).Value = "Region Degraded Total";
            
            // Style headers
            var headerRange = worksheet.Range(row, 1, row, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            
            row++;
            
            // Data rows
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.Location;
                worksheet.Cell(row, 2).Value = item.Score;
                worksheet.Cell(row, 3).Value = item.Lose;
                worksheet.Cell(row, 4).Value = item.Percentage;
                // worksheet.Cell(row, 5).Value = item.Improve.Total;
                // worksheet.Cell(row, 6).Value = item.Degrade.Total;
                row++;
            }
            
            return row;
        }

        private int AddImprovedDetailSection(IXLWorksheet worksheet, List<NewImproveDegradeResponse> data, int startRow)
        {
            int row = startRow;
            
            // Section header
            var sectionHeaderRange = worksheet.Range(row, 1, row, 4);
            sectionHeaderRange.Merge().Value = "Improved Detail";
            sectionHeaderRange.Style.Font.Bold = true;
            sectionHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;
            
            // Column headers
            worksheet.Cell(row, 1).Value = "Location";
            worksheet.Cell(row, 2).Value = "Region";
            worksheet.Cell(row, 3).Value = "Details";
            worksheet.Cell(row, 4).Value = "Status";
            
            var headerRange = worksheet.Range(row, 1, row, 4);
            headerRange.Style.Font.Bold = true;
            row++;
            
            // Data rows with hierarchical structure
            foreach (var location in data)
            {
                var locationStartRow = row;
                bool isFirstRegion = true;
                
                // foreach (var region in location.Improve.Regions)
                // {
                //     var regionStartRow = row;
                //     bool isFirstDetail = true;
                    
                //     foreach (var detail in region.Details ?? new List<string>())
                //     {
                //         if (isFirstRegion && isFirstDetail)
                //         {
                //             worksheet.Cell(row, 1).Value = location.Location;
                //         }
                        
                //         if (isFirstDetail)
                //         {
                //             worksheet.Cell(row, 2).Value = region.Name;
                //             worksheet.Cell(row, 4).Value = region.Status;
                //         }
                        
                //         worksheet.Cell(row, 3).Value = detail;
                        
                //         row++;
                //         isFirstDetail = false;
                //         isFirstRegion = false;
                //     }
                    
                //     // If region has details, merge cells for region and status
                //     if (region.Details?.Count > 1)
                //     {
                //         if (regionStartRow < row - 1)
                //         {
                //             worksheet.Range(regionStartRow, 2, row - 1, 2).Merge();
                //             worksheet.Range(regionStartRow, 4, row - 1, 4).Merge();
                //         }
                //     }
                // }
                
                // Merge location cells
                if (locationStartRow < row - 1)
                {
                    worksheet.Range(locationStartRow, 1, row - 1, 1).Merge();
                }
            }
            
            return row;
        }

        private int AddDegradeDetailSection(IXLWorksheet worksheet, List<NewImproveDegradeResponse> data, int startRow)
        {
            int row = startRow;
            
            // Section header
            var sectionHeaderRange = worksheet.Range(row, 1, row, 4);
            sectionHeaderRange.Merge().Value = "Degrade Detail";
            sectionHeaderRange.Style.Font.Bold = true;
            sectionHeaderRange.Style.Fill.BackgroundColor = XLColor.LightCoral;
            row++;
            
            // Column headers
            worksheet.Cell(row, 1).Value = "Location";
            worksheet.Cell(row, 2).Value = "Region";
            worksheet.Cell(row, 3).Value = "Details";
            worksheet.Cell(row, 4).Value = "Status";
            
            var headerRange = worksheet.Range(row, 1, row, 4);
            headerRange.Style.Font.Bold = true;
            row++;
            
            // Data rows with hierarchical structure
            foreach (var location in data)
            {
                var locationStartRow = row;
                bool isFirstRegion = true;
                
                // foreach (var region in location.Degrade.Regions)
                // {
                //     var regionStartRow = row;
                //     bool isFirstDetail = true;
                    
                //     foreach (var detail in region.Details ?? new List<string>())
                //     {
                //         if (isFirstRegion && isFirstDetail)
                //         {
                //             worksheet.Cell(row, 1).Value = location.Location;
                //         }
                        
                //         if (isFirstDetail)
                //         {
                //             worksheet.Cell(row, 2).Value = region.Name;
                //             worksheet.Cell(row, 4).Value = region.Status;
                //         }
                        
                //         worksheet.Cell(row, 3).Value = detail;
                        
                //         row++;
                //         isFirstDetail = false;
                //         isFirstRegion = false;
                //     }
                    
                //     // If region has details, merge cells for region and status
                //     if (region.Details?.Count > 1)
                //     {
                //         if (regionStartRow < row - 1)
                //         {
                //             worksheet.Range(regionStartRow, 2, row - 1, 2).Merge();
                //             worksheet.Range(regionStartRow, 4, row - 1, 4).Merge();
                //         }
                //     }
                // }
                
                // Merge location cells
                if (locationStartRow < row - 1)
                {
                    worksheet.Range(locationStartRow, 1, row - 1, 1).Merge();
                }
            }
            
            return row;
        }
    }
}
