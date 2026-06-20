using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Infrastructure.Persistence.Repositories;

public class MessageRepository(AppDbContext db) : IMessageRepository
{
    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Messages.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Message>> GetByChatIdAsync(
        Guid chatId,
        Guid? cursor = null,
        int limit = 50,
        CancellationToken ct = default)
    {
        var query = db.Messages
            .Where(x => x.ChatId == chatId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .AsQueryable();

        if (cursor is not null)
        {
            var cursorMessage = await db.Messages.FirstOrDefaultAsync(x => x.Id == cursor, ct);
            if (cursorMessage is not null)
            {
                query = query.Where(x =>
                    x.CreatedAt < cursorMessage.CreatedAt ||
                    (x.CreatedAt == cursorMessage.CreatedAt && x.Id < cursorMessage.Id));
            }
        }

        return await query.Take(limit).ToListAsync(ct);
    }

    public async Task CreateAsync(Message message, CancellationToken ct = default)
    {
        await db.Messages.AddAsync(message, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Message message, CancellationToken ct = default)
    {
        db.Messages.Update(message);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Message message, CancellationToken ct = default)
    {
        message.SoftDelete();
        db.Messages.Update(message);
        await db.SaveChangesAsync(ct);
    }
}
