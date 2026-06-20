using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Infrastructure.Persistence.Repositories;

public class ChatEncryptedKeyRepository(AppDbContext db) : IChatEncryptedKeyRepository
{
    public async Task<ChatEncryptedKey?> GetAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await db.ChatEncryptedKeys.FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserId == userId, ct);
    }

    public async Task UpsertAsync(Guid chatId, Guid userId, string encKey, CancellationToken ct = default)
    {
        var existingKey = await db.ChatEncryptedKeys
            .FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserId == userId, ct);

        if (existingKey is not null)
        {
            existingKey.EncryptedKeyBase64 = encKey;
            db.ChatEncryptedKeys.Update(existingKey);
        }
        else
        {
            var newKey = new ChatEncryptedKey
            {
                ChatId = chatId,
                UserId = userId,
                EncryptedKeyBase64 = encKey,
                CreatedAt = DateTime.UtcNow
            };
            await db.ChatEncryptedKeys.AddAsync(newKey, ct);
        }

        await db.SaveChangesAsync(ct);
    }


    public async Task<IReadOnlyList<ChatEncryptedKey>> GetAllForChatAsync(Guid chatId, CancellationToken ct = default)
    {
        return await db.ChatEncryptedKeys.Where(x => x.ChatId == chatId).ToListAsync(ct);
    }

    public async Task DeleteByUserAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var key = await GetAsync(chatId, userId, ct);
        if (key is not null) db.ChatEncryptedKeys.Remove(key);
        await db.SaveChangesAsync(ct);
    }
}