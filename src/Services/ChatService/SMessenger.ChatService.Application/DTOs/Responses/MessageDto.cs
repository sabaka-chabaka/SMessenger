namespace SMessenger.ChatService.Application.DTOs.Responses;

public record MessageDto(
    Guid Id,
    Guid ChatId,
    Guid SenderId,
    string CiphertextBase64,
    string Nonce,
    bool IsEdited,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? EditedAt
);
