using System.Net;
using System.Text.Json;

namespace BookingService.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                object errorResponse;
                if (_env.IsDevelopment())
                {
                    errorResponse = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = ex.Message,
                        stackTrace = ex.StackTrace,
                        traceId = context.TraceIdentifier
                    };
                }
                else
                {
                    errorResponse = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "An unexpected error occurred. Please try again later.",
                        traceId = context.TraceIdentifier
                    };
                }

                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
