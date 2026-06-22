namespace SMessenger.UserService.Application.DTOs.Requests;

public record BlockUserRequest(
    Guid TargetUserId
);