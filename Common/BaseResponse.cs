using Microsoft.AspNetCore.Mvc;

namespace ExecutiveDashboard.Common
{
    public sealed class BaseResponse
    {
        public int Code { get; init; }
        public bool Success { get; init; }
        public object? Data { get; init; }
        public List<string>? Errors { get; init; }

        public static BaseResponse ToResponse<T>(
            int code,
            bool success,
            T? data,
            IEnumerable<string>? errors
        ) =>
            new BaseResponse
            {
                Code = code,
                Success = success,
                Data = data,
                Errors = errors?.ToList(),
            };
    }
}
