using SMessenger.ChatService.Application.Exceptions;
using SMessenger.ChatService.Application.Interfaces;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Enums;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Application.Services;

public class KeyDistributionService(
    IUserPublicKeyRepository keys,
    IChatEncryptedKeyRepository chatKeys,
    IChatMemberRepository members
) : IKeyDistributionService
{
    public async Task<UserPublicKey> GetPublicKeyAsync(Guid userId, CancellationToken ct = default)
    {
        return await keys.GetByUserIdAsync(userId, ct) ?? throw new KeyNotFoundException();
    }

    public async Task StorePublicKeyAsync(Guid userId, string pubKey, string algorithm, CancellationToken ct = default)
    {
        await keys.UpsertAsync(userId, pubKey, algorithm, ct);
    }

    public async Task<ChatEncryptedKey> GetEncryptedChatKeyAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        if (!await members.IsMemberAsync(chatId, userId, ct))
            throw new NotChatMemberException(chatId, userId);

        return await chatKeys.GetAsync(chatId, userId, ct) ?? throw new ChatNotFoundException(chatId);
    }

    public async Task StoreEncryptedChatKeysAsync(
        Guid chatId,
        Guid callerId,
        IReadOnlyList<(Guid UserId, string EncryptedKeyBase64)> entries,
        CancellationToken ct = default)
    {
        var caller = await members.GetMemberAsync(chatId, callerId, ct)
            ?? throw new NotChatMemberException(chatId, callerId);

        if (!caller.HasRole(MemberRole.Admin))
            throw new NotPermittedException("Required role: Admin");

        foreach (var entry in entries)
        {
            if (!await members.IsMemberAsync(chatId, entry.UserId, ct))
                throw new NotChatMemberException(chatId, entry.UserId);

            await chatKeys.UpsertAsync(chatId, entry.UserId, entry.EncryptedKeyBase64, ct);
        }
    }

    public async Task<IReadOnlyList<UserPublicKey>> GetAllPublicKeysForChatAsync(Guid chatId, CancellationToken ct = default)
    {
        var chatMembers = await members.GetMembersAsync(chatId, ct);
        if (chatMembers.Count == 0) throw new ChatNotFoundException(chatId);

        var userIds = chatMembers.Select(m => m.UserId).ToArray();
        return await keys.GetByUserIdsAsync(userIds, ct);
    }
}
