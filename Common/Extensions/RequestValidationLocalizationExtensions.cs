using ExecutiveDashboard.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ExecutiveDashboard.Common.Extensions
{
    public static class RequestValidationLocalizationExtensions
    {
        public static IServiceCollection AddLocalizedValidationEnvelope(
            this IServiceCollection services
        )
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var localizer = context.HttpContext.RequestServices.GetRequiredService<
                        IStringLocalizer<SharedResources>
                    >();

                    var errors = context
                        .ModelState.SelectMany(kvp =>
                            kvp.Value?.Errors.Select(error =>
                            {
                                var key = !string.IsNullOrWhiteSpace(error.ErrorMessage)
                                    ? error.ErrorMessage
                                    : "invalid_request";
                                return localizer[key].Value;
                            }) ?? Enumerable.Empty<string>()
                        )
                        .ToList();

                    var body = BaseResponse.ToResponse<object?>(400, false, null, errors);
                    return new BadRequestObjectResult(body);
                };
            });

            return services;
        }
    }
}
