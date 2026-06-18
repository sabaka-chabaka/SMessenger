using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Infrastructure.Persistence.Repositories;

public class ChatMemberRepository(AppDbContext db) : IChatMemberRepository
{
    public async Task<IReadOnlyList<ChatMember>> GetMembersAsync(Guid chatId, CancellationToken ct = default)
    {
        return await db.ChatMembers.Where(x => x.ChatId == chatId).ToListAsync(cancellationToken: ct);
    }

    public async Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await db.ChatMembers.FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserId == userId, ct);
    }

    public async Task CreateAsync(ChatMember member, CancellationToken ct = default)
    {
        await db.ChatMembers.AddAsync(member, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var member = await GetMemberAsync(chatId, userId, ct);

        if (member != null) db.ChatMembers.Remove(member);
    }

    public async Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await db.ChatMembers.AnyAsync(x => x.ChatId == chatId && x.UserId == userId, ct);
    }

    public async Task UpdateAsync(ChatMember member, CancellationToken ct = default)
    {
        db.ChatMembers.Update(member);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ILookup<Guid, ChatMember>> GetMembersByChatIdsAsync(
        IReadOnlyList<Guid> chatIds, 
        CancellationToken ct = default)
    {
        if (!chatIds.Any())
        {
            return Enumerable.Empty<ChatMember>().ToLookup(x => Guid.Empty);
        }

        var members = await db.ChatMembers
            .Where(m => chatIds.Contains(m.ChatId))
            .ToListAsync(ct);

        return members.ToLookup(m => m.ChatId);
    }

}