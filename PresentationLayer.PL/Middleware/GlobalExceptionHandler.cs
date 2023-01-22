using BusinessLogic.BAL.Exceptions;
using FluentValidation;

namespace PresentationLayer.PL.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //log exception
                _logger.LogError(ex,"Error from global exception handler, Erorr type: {err}", ex.GetType());

                httpContext.Response.ContentType = "application/json";

                //set default values for response json
                object response = null;

                var statusCode = StatusCodes.Status500InternalServerError;

                if (ex is ForbbidenActionException)
                {
                    statusCode = StatusCodes.Status403Forbidden;
                }
                if (ex is EntityNotFoundException)
                {
                    statusCode = StatusCodes.Status404NotFound;
                }
                if (ex is ConflictedActionException)
                {
                    statusCode = StatusCodes.Status409Conflict;
                    response = new { Error = ex.Message };
                }
                if (ex is ValidationException e)
                {
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    response = new
                    {
                        errors = e.Errors.Select(x => new
                        {
                            errorMessge = x.ErrorMessage,
                            errorProperty = x.PropertyName
                        })
                    };
                }
                // return reposne json obj
                httpContext.Response.StatusCode = statusCode;

                if (response != null)
                {
                    await httpContext.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}
