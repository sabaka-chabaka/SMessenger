namespace SMessenger.ChatService.Application.DTOs.Requests;

public record SendMessageRequest(
    Guid ChatId,
    string CiphertextBase64,
    string Nonce
);