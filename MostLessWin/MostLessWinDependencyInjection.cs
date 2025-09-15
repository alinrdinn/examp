using ExecutiveDashboard.Modules.MostLessWin.Repositories;
using ExecutiveDashboard.Modules.MostLessWin.Services;

namespace ExecutiveDashboard.Modules.MostLessWin
{
    public static class MostLessWinDependencyInjection
    {
        public static IServiceCollection AddMostLessWinModule(this IServiceCollection services)
        {
            services.AddScoped<IMostLessWinService, MostLessWinService>();
            services.AddScoped<IMostLessWinRepository, MostLessWinRepository>();
            return services;
        }
    }
}
