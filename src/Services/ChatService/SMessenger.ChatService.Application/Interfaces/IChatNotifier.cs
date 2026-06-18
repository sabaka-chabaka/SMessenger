namespace SMessenger.ChatService.Application.Interfaces;

public interface IChatNotifier
{
    Task SendToGroupAsync(Guid chatId, Func<IChatClient, Task> action, CancellationToken ct = default);
    Task SendToGroupExceptAsync(Guid chatId, IReadOnlyList<string> excludedConnectionIds, Func<IChatClient, Task> action, CancellationToken ct = default);
    Task SendToUserAsync(Guid userId, Func<IChatClient, Task> action, CancellationToken ct = default);
}