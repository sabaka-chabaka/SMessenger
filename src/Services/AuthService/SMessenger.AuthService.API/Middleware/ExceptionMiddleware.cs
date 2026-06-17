using System.Text.Json;
using SMessenger.AuthService.Application.DTOs.Exceptions;

namespace SMessenger.AuthService.API.Middleware;

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
            UserAlreadyExistsException e  => (StatusCodes.Status409Conflict, e.Message),
            InvalidCredentialsException e => (StatusCodes.Status401Unauthorized, e.Message),
            InvalidRefreshTokenException e => (StatusCodes.Status401Unauthorized, e.Message),
            _                             => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}