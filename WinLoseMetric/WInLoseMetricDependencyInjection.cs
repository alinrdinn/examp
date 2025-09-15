using ExecutiveDashboard.Modules.WinLoseMetric.Repositories;
using ExecutiveDashboard.Modules.WinLoseMetric.Services;

namespace ExecutiveDashboard.Modules.WinLoseMetric
{
    public static class WInLoseMetricDependencyInjection
    {
        public static IServiceCollection AddWinLoseMetricModule(this IServiceCollection services)
        {
            services.AddScoped<IWinLoseMetricService, WinLoseMetricService>();
            services.AddScoped<IWinLoseMetricRepository, WinLoseMetricRepository>();
            return services;
        }
    }
}
