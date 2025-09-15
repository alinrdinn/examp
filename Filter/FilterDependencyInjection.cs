using ExecutiveDashboard.Modules.Filter.Repositories;
using ExecutiveDashboard.Modules.Filter.Services;

namespace ExecutiveDashboard.Modules.ExecutiveDashboard
{
    public static class FilterDependencyInjection
    {
        public static IServiceCollection AddFiltereModule(this IServiceCollection services)
        {
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<IFilterRepository, FilterRepository>();
            return services;
        }
    }
}
