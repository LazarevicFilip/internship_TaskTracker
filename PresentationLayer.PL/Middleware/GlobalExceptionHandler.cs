using Azure;
using BusinessLogic.BAL.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.PL.Middleware
{
    record ErrorResult(int StatusCode, object Response);
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
                //set content-type
                httpContext.Response.ContentType = "application/json";

                ErrorResult result = ex switch
                {
                    UnauthorizedAccessException => new ErrorResult(StatusCodes.Status401Unauthorized, null),

                    ForbiddenActionException => new ErrorResult(StatusCodes.Status403Forbidden, null),

                    EntityNotFoundException => new ErrorResult(StatusCodes.Status404NotFound, null),

                    ConflictedActionException cex => new ErrorResult(StatusCodes.Status409Conflict, new { Error = cex.Message }),

                    DbUpdateConcurrencyException => new ErrorResult(StatusCodes.Status409Conflict, new { Error = "Concurrency violation: The row has been updated or deleted by another transaction. Try again in a moment." }),

                    NotSupportedException => new ErrorResult(StatusCodes.Status409Conflict, new { Error = ex.Message }),

                    RequestFailedException => new ErrorResult(StatusCodes.Status400BadRequest, new { Error = "Error while uploading file to cloud." }),
                    
                    ValidationException e => new ErrorResult(StatusCodes.Status422UnprocessableEntity, new { errors = e.Errors.Select(x => new { errorMessge = x.ErrorMessage, errorProperty = x.PropertyName }) }),

                    //set default values for response json
                    _ => new ErrorResult(StatusCodes.Status500InternalServerError, null)
                };

                // return reposne json obj
                httpContext.Response.StatusCode = result.StatusCode;

                if (result.Response != null)
                {
                    await httpContext.Response.WriteAsJsonAsync(result.Response);
                }
            }
        }
    }
}
