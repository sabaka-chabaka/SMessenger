using System.Security.Claims;
using SMessenger.AuthService.Application.DTOs.Responses;
using SMessenger.AuthService.Domain.Entities;

namespace SMessenger.AuthService.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshTokenData GenerateRefreshToken(); 
    ClaimsPrincipal? ValidateAccessToken(string token);
    Guid GetUserIdFromToken(string token);
}