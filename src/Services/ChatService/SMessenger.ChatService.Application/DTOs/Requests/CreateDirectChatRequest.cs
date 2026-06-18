namespace SMessenger.ChatService.Application.DTOs.Requests;

public record CreateDirectChatRequest(
    Guid OtherUserId,
    string MyEncryptedKeyBase64,
    string OtherUserEncryptedKeyBase64
);