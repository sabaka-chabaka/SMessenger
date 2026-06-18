using SMessenger.ChatService.Application.Interfaces;

namespace SMessenger.ChatService.Application.Services;

public class PresenceService(IPresenceCache presenceCache) : IPresenceService
{
    public async Task UserConnectedAsync(Guid userId, Guid connId, CancellationToken ct = default)
    {
        await presenceCache.SetOnlineAsync(userId, connId, ct);
    }

    public async Task UserDisconnectedAsync(Guid userId, Guid connId, CancellationToken ct = default)
    {
        await presenceCache.RemoveConnectionAsync(userId, connId, ct);
    }

    public async Task<IReadOnlyList<Guid>> GetOnlineUsersAsync(Guid userId, CancellationToken ct = default)
    {
        return await presenceCache.GetOnlineUsersAsync(userId, ct);
    }

    public async Task<bool> IsOnlineAsync(Guid userId, CancellationToken ct = default)
    {
        var onlineConnections = await presenceCache.GetOnlineConnectionsAsync([userId], ct);
        return onlineConnections.Count > 0;
    }
}