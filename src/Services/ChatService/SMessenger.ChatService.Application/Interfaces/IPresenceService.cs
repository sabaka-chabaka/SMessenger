namespace SMessenger.ChatService.Application.Interfaces;

public interface IPresenceService
{
    Task UserConnectedAsync(Guid userId, Guid connId, CancellationToken ct = default);
    Task UserDisconnectedAsync(Guid userId, Guid connId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetOnlineUsersAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsOnlineAsync(Guid userId, CancellationToken ct = default);
}