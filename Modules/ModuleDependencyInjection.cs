using ExecutiveDashboard.Modules.ExcelExport;
using ExecutiveDashboard.Modules.ExecutiveDashboard;
using ExecutiveDashboard.Modules.ImproveDegrade;
using ExecutiveDashboard.Modules.MostLessWin;
using ExecutiveDashboard.Modules.OLOPerformance;
using ExecutiveDashboard.Modules.Summary;
using ExecutiveDashboard.Modules.SummaryNote;
using ExecutiveDashboard.Modules.WinLoseMetric;

namespace ExecutiveDashboard.Modules
{
    public static class ModuleDependencyInjection
    {
        public static IServiceCollection AddModules(this IServiceCollection services)
        {
            services.AddFiltereModule();
            services.AddMostLessWinModule();
            services.AddOLOPerformanceModule();
            services.AddWinLoseMetricModule();
            services.AddSummaryModule();
            services.AddSummaryNoteModule();
            services.AddImproveDegradeModule();
            services.AddExcelExportModule();

            return services;
        }
    }
}
