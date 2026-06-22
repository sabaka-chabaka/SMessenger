namespace SMessenger.UserService.Application.DTOs.Responses;

public record UserBlockDto(
    Guid BlockedUserId,
    DateTime CreatedAt
);