using ExecutiveDashboard.Modules.SummaryNote.Repositories;
using ExecutiveDashboard.Modules.SummaryNote.Services;

namespace ExecutiveDashboard.Modules.ExecutiveDashboard
{
    public static class SummaryNoteDependencyInjection
    {
        public static IServiceCollection AddSummaryNoteModule(this IServiceCollection services)
        {
            services.AddScoped<ISummaryNoteService, SummaryNoteService>();
            services.AddScoped<ISummaryNoteRepository, SummaryNoteRepository>();
            return services;
        }
    }
}
