using Microsoft.EntityFrameworkCore;
using SMessenger.UserService.Domain.Entities;
using SMessenger.UserService.Domain.Repositories;

namespace SMessenger.UserService.Infrastructure.Persistence.Repositories;

public class UserBlockRepository(AppDbContext db) : IUserBlockRepository
{
    public async Task<IReadOnlyList<UserBlock>> GetBlocksByUserAsync(
        Guid blockerUserId, CancellationToken ct = default)
    {
        return await db.UserBlocks
            .Where(x => x.BlockerUserId == blockerUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> IsBlockedAsync(
        Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default)
    {
        return await db.UserBlocks.AnyAsync(
            x => x.BlockerUserId == blockerUserId && x.BlockedUserId == blockedUserId, ct);
    }

    public async Task CreateAsync(UserBlock block, CancellationToken ct = default)
    {
        await db.UserBlocks.AddAsync(block, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default)
    {
        var block = await db.UserBlocks.FirstOrDefaultAsync(
            x => x.BlockerUserId == blockerUserId && x.BlockedUserId == blockedUserId, ct);

        if (block is not null)
        {
            db.UserBlocks.Remove(block);
            await db.SaveChangesAsync(ct);
        }
    }
}