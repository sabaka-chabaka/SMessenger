using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Domain.Interfaces;

public interface IChatRepository
{
    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Chat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default);
    Task CreateAsync(Chat chat, CancellationToken ct = default);
    Task UpdateAsync(Chat chat, CancellationToken ct = default);
}