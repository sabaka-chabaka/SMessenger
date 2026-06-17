namespace SMessenger.AuthService.Application.DTOs.Responses;

public record UserDto(Guid Id, string Email, string Role, DateTime CreatedAt);