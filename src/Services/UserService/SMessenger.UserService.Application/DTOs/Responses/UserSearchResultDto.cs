namespace SMessenger.UserService.Application.DTOs.Responses;

public record UserSearchResultDto(
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl
);