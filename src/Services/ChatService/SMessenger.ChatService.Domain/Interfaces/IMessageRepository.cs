using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Domain.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Message>> GetByChatIdAsync(
        Guid chatId,
        Guid? cursor = null,
        int limit = 50,
        CancellationToken ct = default);
    Task CreateAsync(Message message, CancellationToken ct = default);
    Task UpdateAsync(Message message, CancellationToken ct = default);
    Task SoftDeleteAsync(Message message, CancellationToken ct = default);
}