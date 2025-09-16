using ExecutiveDashboard.Modules.Summary.Repositories;
using ExecutiveDashboard.Modules.Summary.Services;

namespace ExecutiveDashboard.Modules.Summary
{
    public static class SummaryDependencyInjection
    {
        public static IServiceCollection AddSummaryModule(this IServiceCollection services)
        {
            services.AddScoped<ISummaryService, SummaryService>();
            services.AddScoped<ISummaryRepository, SummaryRepository>();
            return services;
        }
    }
}
