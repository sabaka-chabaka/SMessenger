using System.Text.Json;
using SMessenger.ChatService.Application.Exceptions;

namespace SMessenger.ChatService.API.Middleware;

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
            ChatNotFoundException e      => (StatusCodes.Status404NotFound, e.Message),
            MessageNotFoundException e   => (StatusCodes.Status404NotFound, e.Message),
            NotChatMemberException e     => (StatusCodes.Status403Forbidden, e.Message),
            NotPermittedException e      => (StatusCodes.Status403Forbidden, e.Message),
            KeyNotFoundException e       => (StatusCodes.Status404NotFound, e.Message ?? "Key not found"),
            _                            => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}
