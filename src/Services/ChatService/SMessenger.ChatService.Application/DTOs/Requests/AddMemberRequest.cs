namespace SMessenger.ChatService.Application.DTOs.Requests;

public record AddMemberRequest(
    Guid ChatId,
    Guid UserId,
    string EncryptedKeyBase64
);