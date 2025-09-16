using System.Net;
using ExecutiveDashboard.Common;
using ExecutiveDashboard.Common.Exceptions;
using Microsoft.Extensions.Localization;

namespace ExecutiveDashboard.Common.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IStringLocalizer<SharedResources> localizer)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");

                int code = (int)HttpStatusCode.InternalServerError;
                List<string> errors = new();

                switch (ex)
                {
                    case NotFoundException notFoundEx:
                        code = (int)HttpStatusCode.NotFound;
                        errors.Add(localizer[notFoundEx.LocalizationKey].Value);
                        break;
                    case ArgumentException:
                    case FormatException:
                        code = (int)HttpStatusCode.BadRequest;
                        errors.Add(localizer["bad_request"].Value);
                        break;
                    default:
                        errors.Add(localizer["unexpected_error"].Value);
                        break;
                }

                var body = BaseResponse.ToResponse<object?>(
                    code,
                    success: false,
                    data: null,
                    errors
                );
                context.Response.StatusCode = code;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(body);
            }
        }
    }
}
