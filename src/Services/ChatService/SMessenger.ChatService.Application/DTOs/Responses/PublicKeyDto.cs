namespace SMessenger.ChatService.Application.DTOs.Responses;

public record PublicKeyDto(
    Guid UserId,
    string PublicKeyBase64,
    string Algorithm,
    DateTime CreatedAt,
    DateTime UpdatedAt
);