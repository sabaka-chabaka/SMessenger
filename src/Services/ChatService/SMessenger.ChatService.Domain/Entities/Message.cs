namespace SMessenger.ChatService.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid ChatId { get; private set; }
    public Guid SenderId { get; private set; }
    public string CiphertextBase64 { get; private set; } = null!;
    public string Nonce { get; private set; } = null!;
    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }

    private Message() { }

    public static Message Create(Guid chatId, Guid senderId, string ciphertextBase64, string nonce)
        => new()
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            CiphertextBase64 = ciphertextBase64,
            Nonce = nonce,
            CreatedAt = DateTime.UtcNow
        };

    public void Edit(string newCiphertextBase64, string newNonce)
    {
        CiphertextBase64 = newCiphertextBase64;
        Nonce = newNonce;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}