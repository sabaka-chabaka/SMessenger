using Microsoft.EntityFrameworkCore;
using SMessenger.UserService.Domain.Entities;
using SMessenger.UserService.Domain.Repositories;

namespace SMessenger.UserService.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(AppDbContext db) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task<UserProfile?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var normalized = username.ToLowerInvariant();
        return await db.UserProfiles.FirstOrDefaultAsync(x => x.Username == normalized, ct);
    }

    public async Task<IReadOnlyList<UserProfile>> SearchByUsernameAsync(
        string query, int limit = 20, CancellationToken ct = default)
    {
        var normalized = query.ToLowerInvariant();

        return await db.UserProfiles
            .Where(x => x.Username.Contains(normalized))
            .OrderBy(x => x.Username)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserProfile>> GetByUserIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds.Count == 0) return Array.Empty<UserProfile>();
        return await db.UserProfiles.Where(x => userIds.Contains(x.UserId)).ToListAsync(ct);
    }

    public async Task<bool> IsUsernameTakenAsync(string username, CancellationToken ct = default)
    {
        var normalized = username.ToLowerInvariant();
        return await db.UserProfiles.AnyAsync(x => x.Username == normalized, ct);
    }

    public async Task CreateAsync(UserProfile profile, CancellationToken ct = default)
    {
        await db.UserProfiles.AddAsync(profile, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UserProfile profile, CancellationToken ct = default)
    {
        db.UserProfiles.Update(profile);
        await db.SaveChangesAsync(ct);
    }
}