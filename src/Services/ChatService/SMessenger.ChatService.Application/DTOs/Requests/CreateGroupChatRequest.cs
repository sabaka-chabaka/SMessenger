namespace SMessenger.ChatService.Application.DTOs.Requests;

public record CreateGroupChatRequest(
    string Name,
    IEnumerable<MemberKeyRequest> Members
);

public record MemberKeyRequest(
    Guid UserId,
    string EncryptedKeyBase64
);