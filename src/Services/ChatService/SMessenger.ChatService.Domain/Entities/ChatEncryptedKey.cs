namespace SMessenger.ChatService.Domain.Entities;

public class ChatEncryptedKey
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string EncryptedKeyBase64 { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Chat Chat { get; private set; } = null!;
    public UserPublicKey UserPublicKey { get; private set; } = null!;
}