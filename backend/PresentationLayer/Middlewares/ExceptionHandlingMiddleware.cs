using DataAccessLayer.Constants.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace PresentationLayer.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Request aborted by client: {Path}", context.Request.Path);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(ex, "Response already started, cannot write custom error body.");
                    throw;
                }

                var (statusCode, title, detail) = ex switch
                {
                    FluentValidation.ValidationException validation => (
                        StatusCodes.Status400BadRequest,
                        "Validation failed",
                        string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))),

                    KeyNotFoundException notFound => (
                        StatusCodes.Status404NotFound,
                        "Not Found",
                        notFound.Message),

                    ArgumentException or InvalidOperationException => (
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        ex.Message),

                    AppException appEx => (
                        appEx.StatusCode,
                        ReasonPhrases.GetReasonPhrase(appEx.StatusCode),
                        ex.Message),

                    UnauthorizedAccessException unauthorized => (
                        StatusCodes.Status403Forbidden,
                        "Forbidden",
                        unauthorized.Message),

                    OperationCanceledException or TimeoutException => (
                        StatusCodes.Status504GatewayTimeout,
                        "Gateway Timeout",
                        "The request timed out."),

                    _ => (
                        StatusCodes.Status500InternalServerError,
                        "Internal Server Error",
                        "An unexpected error occurred. Please try again later.")
                };

                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    _logger.LogError(ex, "Unhandled server error on {Method} {Path}: {Message}",
                        context.Request.Method, context.Request.Path, ex.Message);
                }
                else
                {
                    _logger.LogInformation("Client request error ({StatusCode}) on {Method} {Path}: {Detail}",
                        statusCode, context.Request.Method, context.Request.Path, detail);
                }

                var problem = new ProblemDetails
                {
                    Type = $"https://httpstatuses.io/{statusCode}",
                    Title = title,
                    Status = statusCode,
                    Detail = detail,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
