using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Domain.Interfaces;

public interface IChatEncryptedKeyRepository
{
    Task<ChatEncryptedKey?> GetAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task UpsertAsync(Guid chatId, Guid userId, string encKey, CancellationToken ct = default);
    Task<IReadOnlyList<ChatEncryptedKey>> GetAllForChatAsync(Guid chatId, CancellationToken ct = default);
    Task DeleteByUserAsync(Guid chatId, Guid userId, CancellationToken ct = default);
}