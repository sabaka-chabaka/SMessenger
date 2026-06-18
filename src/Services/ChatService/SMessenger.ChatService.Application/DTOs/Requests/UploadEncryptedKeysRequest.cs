namespace SMessenger.ChatService.Application.DTOs.Requests;

public record UploadEncryptedKeysRequest(
    IEnumerable<ChatEncryptedKeyRequest> Keys
);

public record ChatEncryptedKeyRequest(
    Guid ChatId,
    string EncryptedKeyBase64
);