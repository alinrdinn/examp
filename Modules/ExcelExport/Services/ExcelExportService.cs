using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using ExecutiveDashboard.Common.Exceptions;
using ExecutiveDashboard.Modules.ExcelExport.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Dtos.Requests;
using ExecutiveDashboard.Modules.ImproveDegrade.Services;
using ExecutiveDashboard.Modules.MostLessWin.Dtos.Requests;
using ExecutiveDashboard.Modules.MostLessWin.Services;
using ExecutiveDashboard.Modules.OLOPerformance.Dtos.Requests;
using ExecutiveDashboard.Modules.OLOPerformance.Services;
using ExecutiveDashboard.Modules.Summary.Dtos.Requests;
using ExecutiveDashboard.Modules.Summary.Services;
using ExecutiveDashboard.Modules.SummaryNote.Dtos.Requests;
using ExecutiveDashboard.Modules.SummaryNote.Services;
using ExecutiveDashboard.Modules.WinLoseMetric.Dtos.Requests;
using ExecutiveDashboard.Modules.WinLoseMetric.Services;

namespace ExecutiveDashboard.Modules.ExcelExport.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private static readonly string[] Sources = new[] { "open_signal", "ookla" };
        private static readonly string[] Statuses = new[] { "lose", "win" };

        private readonly IWinLoseMetricService _winLoseMetricService;
        private readonly IMostLessWinService _mostLessWinService;
        private readonly IImproveDegradeService _improveDegradeService;
        private readonly ISummaryService _summaryService;
        private readonly ISummaryNoteService _summaryNoteService;
        private readonly IOLOPerformanceService _oloPerformanceService;

        public ExcelExportService(
            IWinLoseMetricService winLoseMetricService,
            IMostLessWinService mostLessWinService,
            IImproveDegradeService improveDegradeService,
            ISummaryService summaryService,
            ISummaryNoteService summaryNoteService,
            IOLOPerformanceService oloPerformanceService
        )
        {
            _winLoseMetricService = winLoseMetricService;
            _mostLessWinService = mostLessWinService;
            _improveDegradeService = improveDegradeService;
            _summaryService = summaryService;
            _summaryNoteService = summaryNoteService;
            _oloPerformanceService = oloPerformanceService;
        }

        public async Task<XLWorkbook> GenerateExcelExportWorkbook(ExcelExportRequest request)
        {
            var workbook = new XLWorkbook();
            var hasWorksheet = false;

            async Task TryAddWorksheet(Func<Task<IXLWorksheet>> createWorksheet)
            {
                try
                {
                    await createWorksheet();
                    hasWorksheet = true;
                }
                catch (NotFoundException)
                {
                    // Skip sheets without data
                }
            }

            var summaryRequest = new SummaryRequest
            {
                Yearweek = request.Yearweek,
                Level = request.Level,
                Location = request.Location,
            };

            await TryAddWorksheet(() =>
                _summaryService.CreateSummaryWorksheet(workbook, summaryRequest, "Summary")
            );

            await TryAddWorksheet(() => _summaryNoteService.CreateSummaryNotesWorksheet(
                workbook,
                new SummaryNoteQueryRequest { Yearweek = request.Yearweek },
                "Summary Notes"
            ));

            var oloRequest = new OLOPerformanceRequest
            {
                Yearweek = request.Yearweek,
                Level = request.Level,
                Location = request.Location,
            };

            await TryAddWorksheet(() =>
                _oloPerformanceService.CreateOloPerformanceWorksheet(
                    workbook,
                    oloRequest,
                    "OLO Performance"
                )
            );

            await TryAddWorksheet(() =>
                _oloPerformanceService.CreateOloPerformanceSummaryWorksheet(
                    workbook,
                    oloRequest,
                    "OLO Performance Summary"
                )
            );

            foreach (var source in Sources)
            {
                var formattedSource = ToTitleCase(source);

                await TryAddWorksheet(() =>
                    _mostLessWinService.CreateWinLoseMetricsWorksheet(
                        workbook,
                        new MostLessWinRequest
                        {
                            Yearweek = request.Yearweek,
                            Level = request.Level,
                            Location = request.Location,
                            Source = source,
                        },
                        $"Most Less Win {formattedSource}"
                    )
                );

                await TryAddWorksheet(() =>
                    _improveDegradeService.CreateWinLoseWorksheet(
                        workbook,
                        new ImproveDegradeRequest
                        {
                            Yearweek = request.Yearweek,
                            Source = source,
                        },
                        $"Improve Degrade {formattedSource}"
                    )
                );

                foreach (var status in Statuses)
                {
                    var worksheetName =
                        $"Win Lose {formattedSource} {ToTitleCase(status)}";

                    await TryAddWorksheet(() =>
                        _winLoseMetricService.CreateWinLoseMetricsWorksheet(
                            workbook,
                            new WinLoseMetricRequest
                            {
                                Yearweek = request.Yearweek,
                                Level = request.Level,
                                Location = request.Location,
                                Source = source,
                                Status = status,
                            },
                            worksheetName
                        )
                    );
                }
            }

            if (!hasWorksheet)
            {
                throw new NotFoundException("not_found");
            }

            return workbook;
        }

        public async Task<byte[]> GenerateExcelExportFile(ExcelExportRequest request)
        {
            using var workbook = await GenerateExcelExportWorkbook(request);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string ToTitleCase(string value)
        {
            var normalized = value.Replace("_", " ");
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }
    }
}
