namespace SMessenger.AuthService.Application.DTOs.Responses;

public record RefreshTokenData(string Token, DateTime ExpiresAt);