using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace Presentation.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = e switch
            {
                EntityAlreadyExistsException => (int)HttpStatusCode.Conflict,
                EntityNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidPropertyFormatException or InvalidPropertyValueException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            if (context.Response.StatusCode is not (int)HttpStatusCode.InternalServerError)
                logger.LogWarning(e, "Expected error: {error}", e.Message);
            else
                logger.LogError(e, "Unexpected error: {error}", e.Message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { e.Message }));
        }
    }
}