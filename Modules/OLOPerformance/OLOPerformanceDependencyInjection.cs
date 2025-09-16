using ExecutiveDashboard.Modules.OLOPerformance.Repositories;
using ExecutiveDashboard.Modules.OLOPerformance.Services;

namespace ExecutiveDashboard.Modules.OLOPerformance
{
    public static class WInLoseMetricDependencyInjection
    {
        public static IServiceCollection AddOLOPerformanceModule(this IServiceCollection services)
        {
            services.AddScoped<IOLOPerformanceRepository, OLOPerformanceRepository>();
            services.AddScoped<IOLOPerformanceService, OLOPerformanceService>();
            return services;
        }
    }
}
