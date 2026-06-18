using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Infrastructure.Persistence.Repositories;

public class ChatRepository(AppDbContext db) : IChatRepository
{
    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Chats.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Chat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.Chats.Where(x => x.Members.Any(m => m.UserId == userId)).ToListAsync(ct);
    }

    public async Task CreateAsync(Chat chat, CancellationToken ct = default)
    {
        await db.Chats.AddAsync(chat, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Chat chat, CancellationToken ct = default)
    {
        db.Chats.Update(chat);
        await db.SaveChangesAsync(ct);
    }
}