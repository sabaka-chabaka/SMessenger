namespace SMessenger.AuthService.Application.DTOs.Responses;

public record AuthResult(string AccessToken, string RefreshToken);