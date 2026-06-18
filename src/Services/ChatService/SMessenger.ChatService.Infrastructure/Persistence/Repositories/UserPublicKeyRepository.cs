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

    public async Task UpsertAsync(Guid userId, string publicKey, CancellationToken ct = default)
    {
        var existingKey = await db.UserPublicKeys
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (existingKey is not null)
        {
            existingKey.PublicKeyBase64 = publicKey;
            db.UserPublicKeys.Update(existingKey);
        }
        else
        {
            var newKey = new UserPublicKey
            {
                UserId = userId,
                PublicKeyBase64 = publicKey
            };
            await db.UserPublicKeys.AddAsync(newKey, ct);
        }

        await db.SaveChangesAsync(ct);
    }

}