using SMessenger.ChatService.Application.Interfaces;
using StackExchange.Redis;

namespace SMessenger.ChatService.Infrastructure.Presence;

public class RedisPresenceCache(IConnectionMultiplexer redis) : IPresenceCache
{
    private readonly IDatabase _db = redis.GetDatabase();

    private static string Key(Guid userId) => $"presence:{userId}";

    public async Task SetOnlineAsync(Guid userId, Guid connId, CancellationToken ct = default)
    {
        await _db.HashSetAsync(
            Key(userId),
            connId.ToString(),
            DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        );
    }

    public async Task RemoveConnectionAsync(Guid userId, Guid connId, CancellationToken ct = default)
    {
        await _db.HashDeleteAsync(Key(userId), connId.ToString());
    }

    public async Task<IReadOnlyList<Guid>> GetOnlineUsersAsync(Guid userId, CancellationToken ct = default)
    {
        var fields = await _db.HashKeysAsync(Key(userId));
        return fields
            .Where(f => f.HasValue)
            .Select(f => Guid.Parse(f.ToString()))
            .ToList();
    }

    public async Task<IReadOnlyList<Guid>> GetOnlineConnectionsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default)
    {
        var tasks = userIds.Select(uid => _db.HashLengthAsync(Key(uid)));
        var lengths = await Task.WhenAll(tasks);

        return userIds
            .Zip(lengths, (uid, len) => (uid, len))
            .Where(x => x.len > 0)
            .Select(x => x.uid)
            .ToList();
    }
}