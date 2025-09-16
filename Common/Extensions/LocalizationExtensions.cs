using System.Globalization;
using ExecutiveDashboard.Common.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ExecutiveDashboard.Common.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddAppLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer<SharedResources>, SimpleLocalizationService>();

            return services;
        }

        public static IApplicationBuilder UseAppRequestLocalization(this IApplicationBuilder app)
        {
            var cultures = new[] { new CultureInfo("en") };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = cultures.ToList(),
                SupportedUICultures = cultures.ToList(),
            };

            return app.UseRequestLocalization(options);
        }
    }
}
