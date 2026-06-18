using SMessenger.ChatService.Application.Exceptions;
using SMessenger.ChatService.Application.Interfaces;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Application.Services;

public class KeyDistributionService(IUserPublicKeyRepository keys, IChatEncryptedKeyRepository chatKeys, IChatMemberRepository members) : IKeyDistributionService
{
    public async Task<UserPublicKey> GetPublicKeyAsync(Guid userId, CancellationToken ct = default)
    {
        return await keys.GetByUserIdAsync(userId, ct) ?? throw new KeyNotFoundException();
    }

    public async Task StorePublicKeyAsync(Guid userId, string pubKey, CancellationToken ct = default)
    {
        await keys.UpsertAsync(userId, pubKey, ct);
    }

    public async Task<ChatEncryptedKey> GetEncryptedChatKeyAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await chatKeys.GetAsync(chatId, userId, ct) ?? throw new ChatNotFoundException(chatId);
    }

    public async Task StoreEncryptedChatKeyAsync(Guid chatId, Guid userId, string encKey, CancellationToken ct = default)
    {
        await chatKeys.UpsertAsync(chatId, userId, encKey, ct);
    }

    public async Task<IReadOnlyList<UserPublicKey>> GetAllPublicKeysForChatAsync(Guid chatId, CancellationToken ct = default)
    {
        var chatMembers = await members.GetMembersAsync(chatId, ct);
        if (chatMembers is null) throw new ChatNotFoundException(chatId);

        var publicKeys = new List<UserPublicKey>();

        foreach (var member in chatMembers)
        {
            var key = await keys.GetByUserIdAsync(member.UserId, ct);
            if (key is not null)
            {
                publicKeys.Add(key);
            }
        }

        return publicKeys;
    }
}