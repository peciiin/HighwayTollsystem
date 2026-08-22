using Microsoft.AspNetCore.Diagnostics;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;

namespace HighwayTollsystem.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }


        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(ex, "An error occurred while processing your request.");
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Detail = ex.Message,
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = details.Status.Value;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(details, cancellationToken: cancellationToken);
            return true;

        }

    }
}
