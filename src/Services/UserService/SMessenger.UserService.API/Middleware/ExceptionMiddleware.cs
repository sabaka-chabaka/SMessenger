using System.Text.Json;
using SMessenger.UserService.Application.Exceptions;

namespace SMessenger.UserService.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            UserNotFoundException e      => (StatusCodes.Status404NotFound, e.Message),
            UsernameTakenException e     => (StatusCodes.Status409Conflict, e.Message),
            UserBlockedException e       => (StatusCodes.Status403Forbidden, e.Message),
            CannotBlockSelfException e   => (StatusCodes.Status400BadRequest, e.Message),
            _                            => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}