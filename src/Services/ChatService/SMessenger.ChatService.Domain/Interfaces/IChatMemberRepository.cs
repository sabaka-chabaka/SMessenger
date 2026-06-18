using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Domain.Interfaces;

public interface IChatMemberRepository
{
    Task<IReadOnlyList<ChatMember>> GetMembersAsync(Guid chatId, CancellationToken ct = default);
    Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task CreateAsync(ChatMember member, CancellationToken ct = default);
    Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default);
}