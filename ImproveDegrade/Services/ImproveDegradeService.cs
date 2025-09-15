using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Responses;
using ExecutiveDashboard.Modules.ImproveDegrade.Repositories;
using ClosedXML.Excel;
using System.Drawing;

namespace ExecutiveDashboard.Modules.ImproveDegrade.Services
{
    public class ImproveDegradeService : IImproveDegradeService
    {
        private readonly IImproveDegradeRepository _repo;

        public ImproveDegradeService(IImproveDegradeRepository repo)
        {
            _repo = repo;
        }


        public async Task<List<ImproveDegradeResponse>> GetImproveDegrade(ImproveDegradeRequest request)
        {
            var rows = await _repo.GetAreaWinLose(request.Yearweek!.Value, "nation", "nationwide", request.Source!);

            var data = new List<ImproveDegradeResponse>();

            foreach (var r in rows)
            {
                var detail = new ImproveDegradeResponse
                {
                    Location = r.location,
                    Score = r.win ?? 0,
                    Lose = r.lose ?? 0,
                    Percentage = r.percentage,
                    Improve = BuildHardcodedImprove(),
                    Degrade = BuildHardcodedDegrade()
                };

                data.Add(detail);
            }

            return data;
        }

        private static ImproveGroup BuildHardcodedImprove()
        {
            var items = new List<RegionItem>
            {
                new RegionItem
                {
                    Name = "SUMBAGTENG",
                    Details = new List<string>
                    {
                        "Voice App Exp 2 to 3 (0,16)",
                        "low Location: Medan - 20.10 Point"
                    },
                    Status = "Menaikan Score kota Medan dan Aceh"
                },
                new RegionItem
                {
                    Name = "SUMBAGTENG",
                    Details = new List<string>
                    {
                        "Voice App Exp 2 to 3 (0,16)",
                        "low Location: Medan - 20.10 Point"
                    },
                    Status = "Menaikan Score kota Medan dan Aceh"
                }
            };

            return new ImproveGroup
            {
                Total = 10, // Changed to hardcoded value 10 as per requirement
                Regions = items
            };
        }

        private static DegradeGroup BuildHardcodedDegrade()
        {
            var items = new List<RegionItem>
            {
                new RegionItem
                {
                    Name = "SUMBAGTENG",
                    Details = new List<string>
                    {
                        "Voice App Exp 2 to 3 (0,16)",
                        "low Location: Medan - 20.10 Point"
                    },
                    Status = "Menaikan Score kota Medan dan Aceh"
                },
                new RegionItem
                {
                    Name = "JATIM",
                    Details = new List<string>
                    {
                        "Voice App Exp 2 to 3 (0,16)",
                        "low Location: Medan - 20.10 Point"
                    },
                    Status = "Menaikan Score kota Medan dan Aceh"
                }
            };

            return new DegradeGroup
            {
                Total = 10, // Changed to hardcoded value 10 as per requirement
                Regions = items
            };
        }

        public async Task<XLWorkbook> GenerateWinLoseWorkbook(ImproveDegradeRequest request)
        {
            // Get data from existing service method
            var data = await GetImproveDegrade(request);
            
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ImproveDegrade Report");
            
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
            
            worksheet.Cell(row, 1).Value = "Level";
            worksheet.Cell(row, 2).Value = "nation";
            row++;
            
            worksheet.Cell(row, 1).Value = "Location";
            worksheet.Cell(row, 2).Value = "nationwide";
            row++;
            
            worksheet.Cell(row, 1).Value = "Source";
            var sourceValue = request.Source == "open_signal" ? "os" : request.Source;
            worksheet.Cell(row, 2).Value = sourceValue;
            row++;
            
            return row;
        }

        private int AddSummaryStatisticsSection(IXLWorksheet worksheet, List<ImproveDegradeResponse> data, int startRow)
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
                worksheet.Cell(row, 5).Value = item.Improve.Total;
                worksheet.Cell(row, 6).Value = item.Degrade.Total;
                row++;
            }
            
            return row;
        }

        private int AddImprovedDetailSection(IXLWorksheet worksheet, List<ImproveDegradeResponse> data, int startRow)
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
                
                foreach (var region in location.Improve.Regions)
                {
                    var regionStartRow = row;
                    bool isFirstDetail = true;
                    
                    foreach (var detail in region.Details ?? new List<string>())
                    {
                        if (isFirstRegion && isFirstDetail)
                        {
                            worksheet.Cell(row, 1).Value = location.Location;
                        }
                        
                        if (isFirstDetail)
                        {
                            worksheet.Cell(row, 2).Value = region.Name;
                            worksheet.Cell(row, 4).Value = region.Status;
                        }
                        
                        worksheet.Cell(row, 3).Value = detail;
                        
                        row++;
                        isFirstDetail = false;
                        isFirstRegion = false;
                    }
                    
                    // If region has details, merge cells for region and status
                    if (region.Details?.Count > 1)
                    {
                        if (regionStartRow < row - 1)
                        {
                            worksheet.Range(regionStartRow, 2, row - 1, 2).Merge();
                            worksheet.Range(regionStartRow, 4, row - 1, 4).Merge();
                        }
                    }
                }
                
                // Merge location cells
                if (locationStartRow < row - 1)
                {
                    worksheet.Range(locationStartRow, 1, row - 1, 1).Merge();
                }
            }
            
            return row;
        }

        private int AddDegradeDetailSection(IXLWorksheet worksheet, List<ImproveDegradeResponse> data, int startRow)
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
                
                foreach (var region in location.Degrade.Regions)
                {
                    var regionStartRow = row;
                    bool isFirstDetail = true;
                    
                    foreach (var detail in region.Details ?? new List<string>())
                    {
                        if (isFirstRegion && isFirstDetail)
                        {
                            worksheet.Cell(row, 1).Value = location.Location;
                        }
                        
                        if (isFirstDetail)
                        {
                            worksheet.Cell(row, 2).Value = region.Name;
                            worksheet.Cell(row, 4).Value = region.Status;
                        }
                        
                        worksheet.Cell(row, 3).Value = detail;
                        
                        row++;
                        isFirstDetail = false;
                        isFirstRegion = false;
                    }
                    
                    // If region has details, merge cells for region and status
                    if (region.Details?.Count > 1)
                    {
                        if (regionStartRow < row - 1)
                        {
                            worksheet.Range(regionStartRow, 2, row - 1, 2).Merge();
                            worksheet.Range(regionStartRow, 4, row - 1, 4).Merge();
                        }
                    }
                }
                
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
