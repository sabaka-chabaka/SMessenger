using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IKeyDistributionService
{
    Task<UserPublicKey> GetPublicKeyAsync(Guid userId, CancellationToken ct = default);
    Task StorePublicKeyAsync(Guid userId, string pubKey, string algorithm, CancellationToken ct = default);
    Task<ChatEncryptedKey> GetEncryptedChatKeyAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task StoreEncryptedChatKeysAsync(
        Guid chatId,
        Guid callerId,
        IReadOnlyList<(Guid UserId, string EncryptedKeyBase64)> entries,
        CancellationToken ct = default);
    Task<IReadOnlyList<UserPublicKey>> GetAllPublicKeysForChatAsync(Guid chatId, CancellationToken ct = default);
}