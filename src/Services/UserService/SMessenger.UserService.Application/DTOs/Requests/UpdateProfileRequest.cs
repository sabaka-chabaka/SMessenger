namespace SMessenger.UserService.Application.DTOs.Requests;

public record UpdateProfileRequest(
    string DisplayName,
    string? Bio
);