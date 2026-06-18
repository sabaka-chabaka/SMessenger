using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Domain.Entities;

public class ChatMember
{
    public Guid ChatId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LastReadAt { get; private set; }

    private ChatMember() { }

    public static ChatMember Create(Guid chatId, Guid userId, MemberRole role)
        => new() { ChatId = chatId, UserId = userId, Role = role, JoinedAt = DateTime.UtcNow };

    public void UpdateLastRead(DateTime readAt)
        => LastReadAt = readAt;

    public bool HasRole(params MemberRole[] roles)
        => roles.Contains(Role);
}