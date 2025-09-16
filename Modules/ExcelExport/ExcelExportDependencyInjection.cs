using ExecutiveDashboard.Modules.ExcelExport.Services;

namespace ExecutiveDashboard.Modules.ExcelExport
{
    public static class ExcelExportDependencyInjection
    {
        public static IServiceCollection AddExcelExportModule(this IServiceCollection services)
        {
            services.AddScoped<IExcelExportService, ExcelExportService>();
            return services;
        }
    }
}
