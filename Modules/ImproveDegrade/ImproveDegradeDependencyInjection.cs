using ExecutiveDashboard.Modules.ImproveDegrade.Repositories;
using ExecutiveDashboard.Modules.ImproveDegrade.Services;

namespace ExecutiveDashboard.Modules.ImproveDegrade
{
    public static class ImproveDegradeDependencyInjection
    {
        public static IServiceCollection AddImproveDegradeModule(this IServiceCollection services)
        {
            services.AddScoped<IImproveDegradeService, ImproveDegradeService>();
            services.AddScoped<IImproveDegradeRepository, ImproveDegradeRepository>();
            return services;
        }
    }
}
