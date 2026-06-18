using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Domain.Entities;

public class ChatMember
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime LastReadAt { get; set; }
}