using DataAccessLayer.Constants.Exceptions;
using DataAccessLayer.Constants.Messages;
using FluentValidation;
using PresentationLayer.Common;

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
            try {
                await _next(context);
            } catch(ValidationException ex) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<object?>(false, ex.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation failed.", null);

                await context.Response.WriteAsJsonAsync(response);
            } catch(AppException ex) {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object?> (false, ex.Message, null);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, StatusCode.InternalServerError500);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object?>(false, StatusCode.InternalServerError500, null);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
