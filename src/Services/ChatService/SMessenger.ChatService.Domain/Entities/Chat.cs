using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Domain.Entities;

public class Chat
{
    public Guid Id { get; private set; }
    public ChatType Type { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Chat() { }

    public static Chat Create(ChatType type, string name = "")
        => new() { Id = Guid.NewGuid(), Type = type, Name = name, CreatedAt = DateTime.UtcNow };
}