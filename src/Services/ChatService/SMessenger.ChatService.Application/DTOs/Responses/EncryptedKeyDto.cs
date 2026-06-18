namespace SMessenger.ChatService.Application.DTOs.Responses;

public record EncryptedKeyDto(
    Guid ChatId,
    Guid UserId,
    string EncryptedKeyBase64,
    DateTime CreatedAt
);