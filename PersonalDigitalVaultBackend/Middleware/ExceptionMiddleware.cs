using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Common;
using System.Net;
using System.Text.Json;

namespace PersonalDigitalVaultBackend.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    FileNotFoundException => (int)HttpStatusCode.NotFound,
                    InvalidOperationException => (int)HttpStatusCode.BadRequest,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var friendlyMessage = ex is FileNotFoundException
                    ? "That file is missing from storage on this server - it may have been moved, or the database and file storage are out of sync (e.g. after redeploying without the App_Data/VaultFiles folder)."
                    : ex.Message;

                var response = ApiResponseDto<object>.Fail(
                    context.Response.StatusCode == 500 ? "An unexpected error occurred." : friendlyMessage);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            }
        }
    }
}
