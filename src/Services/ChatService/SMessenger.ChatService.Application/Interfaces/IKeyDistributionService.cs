using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IKeyDistributionService
{
    Task<UserPublicKey> GetPublicKeyAsync(Guid userId);
    Task StorePublicKeyAsync(Guid userId, string pubKey);
    Task<ChatEncryptedKey> GetEncryptedChatKeyAsync(Guid chatId, Guid userId);
    Task StoreEncryptedChatKeyAsync(Guid chatId, Guid userId, string encKey);
    Task<IReadOnlyList<UserPublicKey>> GetAllPublicKeysForChatAsync(Guid chatId);
}