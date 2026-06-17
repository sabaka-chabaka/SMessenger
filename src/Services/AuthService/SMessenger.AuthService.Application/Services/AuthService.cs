using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using SMessenger.AuthService.Application.DTOs.Exceptions;
using SMessenger.AuthService.Application.DTOs.Requests;
using SMessenger.AuthService.Application.DTOs.Responses;
using SMessenger.AuthService.Application.Interfaces;
using SMessenger.AuthService.Domain.Entities;
using SMessenger.AuthService.Domain.Interfaces;

namespace SMessenger.AuthService.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher, IConfiguration config
) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await userRepository.ExistsAsync(request.Email, ct)) throw new UserAlreadyExistsException(request.Email);
        if (request.Key != config["SecretSettings:Secret"]) throw new InvalidCredentialsException();
        
        var user = User.Create(request.Email, passwordHasher.Hash(request.Password));
        
        await userRepository.CreateAsync(user, ct);
        
        var token = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        await refreshTokenRepository.CreateAsync(RefreshToken.Create(user.Id, refreshToken.Token, refreshToken.ExpiresAt), ct);
        
        return new AuthResult(token, refreshToken.Token);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenData = tokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenData.Token,
            refreshTokenData.ExpiresAt
        );

        await refreshTokenRepository.CreateAsync(refreshToken, ct);

        return new AuthResult(accessToken, refreshTokenData.Token);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var existing = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);

        if (existing is null || !existing.IsActive)
            throw new InvalidRefreshTokenException();

        var user = await userRepository.GetByIdAsync(existing.UserId, ct);

        existing.Revoke();
        await refreshTokenRepository.UpdateAsync(existing, ct);

        var newAccessToken = tokenService.GenerateAccessToken(user!);
        var newRefreshTokenData = tokenService.GenerateRefreshToken();

        var newRefreshToken = RefreshToken.Create(
            user!.Id,
            newRefreshTokenData.Token,
            newRefreshTokenData.ExpiresAt
        );

        await refreshTokenRepository.CreateAsync(newRefreshToken, ct);

        return new AuthResult(newAccessToken, newRefreshTokenData.Token);
    }

    public async Task RevokeAsync(Guid userId, CancellationToken ct = default)
    {
        await refreshTokenRepository.RevokeAllByUserIdAsync(userId, ct);
    }

    public async Task<UserDto> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        return user == null ? throw new InvalidCredentialsException() : new UserDto(user.Id, user.Email, user.Role, user.CreatedAt);
    }
}