namespace SMessenger.ChatService.Application.Interfaces;

public interface IPresenceCache
{
    Task SetOnlineAsync(Guid userId, Guid connId, CancellationToken ct = default);
    Task RemoveConnectionAsync(Guid userId, Guid connId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetOnlineUsersAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetOnlineConnectionsAsync(IReadOnlyList<Guid> userIds,CancellationToken ct = default);
}