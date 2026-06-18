using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Domain.Entities;

public class Chat
{
    public Guid Id { get;  set; }
    public ChatType Type { get;  set; }
    public string Name { get;  set; } = null!;
    public DateTime CreatedAt { get;  set; }
}