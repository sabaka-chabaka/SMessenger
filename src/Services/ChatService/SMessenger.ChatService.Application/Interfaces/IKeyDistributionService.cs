using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IKeyDistributionService
{
    Task<UserPublicKey> GetPublicKeyAsync(Guid userId, CancellationToken ct = default);
    Task StorePublicKeyAsync(Guid userId, string pubKey, CancellationToken ct = default);
    Task<ChatEncryptedKey> GetEncryptedChatKeyAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task StoreEncryptedChatKeyAsync(Guid chatId, Guid userId, string encKey, CancellationToken ct = default);
    Task<IReadOnlyList<UserPublicKey>> GetAllPublicKeysForChatAsync(Guid chatId, CancellationToken ct = default);
}