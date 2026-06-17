using System.Security.Claims;
using SMessenger.AuthService.Domain.Entities;

namespace SMessenger.AuthService.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateAccessToken(string token);
    Guid GetUserIdFromToken(string token);
}