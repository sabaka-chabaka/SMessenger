namespace SMessenger.ChatService.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChatId { get; private set; }
    public Guid SenderId { get; private set; }
    public string CiphertextBase64 { get; private set; } = null!;
    public string Nonce { get; private set; } = null!;
    public bool IsEdited { get; private set; } = false;
    public bool IsDeleted { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime EditedAt { get; private set; } = DateTime.UtcNow;
}