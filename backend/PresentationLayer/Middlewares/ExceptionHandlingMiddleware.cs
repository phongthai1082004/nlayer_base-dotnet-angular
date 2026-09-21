using DataAccessLayer.Constants.Exceptions;
using DataAccessLayer.Constants.Messages;

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
            } catch(AppException ex) {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    code = ex.StatusCode,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, InternalErrorMessages.InternalServerError);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    code = StatusCodes.Status500InternalServerError,
                    message = InternalErrorMessages.InternalServerError
                });
            }
        }
    }
}
