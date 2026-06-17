using Microsoft.EntityFrameworkCore;
using SMessenger.AuthService.Domain.Entities;
using SMessenger.AuthService.Domain.Interfaces;

namespace SMessenger.AuthService.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task CreateAsync(User user, CancellationToken ct = default)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
    {
        return await db.Users.AnyAsync(x => x.Email == email, ct);
    }
}