namespace SMessenger.ChatService.Application.DTOs.Requests;

public record EditMessageRequest(
    Guid MessageId,
    string NewCiphertextBase64,
    string NewNonce
);