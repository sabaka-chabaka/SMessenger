namespace SMessenger.AuthService.Infrastructure.Settings;

public class JwtSettings
{
    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpireMin { get; init; } = 15;
    public int RefreshTokenExpireDays { get; init; } = 30;
}