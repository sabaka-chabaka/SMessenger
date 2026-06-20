namespace SMessenger.ChatService.Application.DTOs.Requests;

public record UploadEncryptedKeysRequest(
    IEnumerable<ChatEncryptedKeyEntry> EncryptedKeys
);

public record ChatEncryptedKeyEntry(
    Guid UserId,
    string EncryptedKeyBase64
);
