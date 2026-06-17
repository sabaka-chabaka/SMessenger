namespace SMessenger.AuthService.Application.DTOs.Exceptions;

public class InvalidRefreshTokenException() 
    : Exception("Refresh token is invalid or expired");