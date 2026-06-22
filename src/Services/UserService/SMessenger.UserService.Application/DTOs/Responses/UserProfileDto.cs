namespace SMessenger.UserService.Application.DTOs.Responses;

public record UserProfileDto(
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    DateTime? LastSeenAt,
    bool ShowLastSeen
);