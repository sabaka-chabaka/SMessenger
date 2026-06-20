using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Infrastructure.Persistence.Repositories;

public class UserPublicKeyRepository(AppDbContext db) : IUserPublicKeyRepository
{
    public async Task<UserPublicKey?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.UserPublicKeys.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<UserPublicKey>> GetByUserIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds.Count == 0) return Array.Empty<UserPublicKey>();
        return await db.UserPublicKeys.Where(x => userIds.Contains(x.UserId)).ToListAsync(ct);
    }

    public async Task UpsertAsync(Guid userId, string publicKey, string algorithm, CancellationToken ct = default)
    {
        var existingKey = await db.UserPublicKeys
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        var now = DateTime.UtcNow;

        if (existingKey is not null)
        {
            existingKey.PublicKeyBase64 = publicKey;
            existingKey.Algorithm = algorithm;
            existingKey.UpdatedAt = now;
            db.UserPublicKeys.Update(existingKey);
        }
        else
        {
            var newKey = new UserPublicKey
            {
                UserId = userId,
                PublicKeyBase64 = publicKey,
                Algorithm = algorithm,
                CreatedAt = now,
                UpdatedAt = now
            };
            await db.UserPublicKeys.AddAsync(newKey, ct);
        }

        await db.SaveChangesAsync(ct);
    }
}
