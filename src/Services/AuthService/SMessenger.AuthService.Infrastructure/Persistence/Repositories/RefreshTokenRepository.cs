using Microsoft.EntityFrameworkCore;
using SMessenger.AuthService.Domain.Entities;
using SMessenger.AuthService.Domain.Interfaces;

namespace SMessenger.AuthService.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow).ToListAsync(ct);
    }

    public async Task CreateAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await db.RefreshTokens.AddAsync(refreshToken, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        db.RefreshTokens.Update(refreshToken);
        await db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var activeTokens = await db.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync(ct);
        
        if (activeTokens.Count == 0) return;

        foreach (var token in activeTokens) token.Revoke();

        await db.SaveChangesAsync(ct);
    }
}
