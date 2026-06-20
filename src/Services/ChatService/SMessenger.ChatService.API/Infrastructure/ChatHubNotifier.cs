using Microsoft.AspNetCore.SignalR;
using SMessenger.ChatService.API.Hubs;
using SMessenger.ChatService.Application.DTOs.Responses;
using SMessenger.ChatService.Application.Interfaces;

namespace SMessenger.ChatService.API.Infrastructure;

public class ChatHubNotifier(IHubContext<ChatHub, IChatClient> hub) : IChatNotifier
{
    public Task SendToGroupAsync(Guid chatId, Func<IChatClient, Task> action, CancellationToken ct = default)
        => action(hub.Clients.Group(chatId.ToString()));

    public Task SendToGroupExceptAsync(
        Guid chatId,
        IReadOnlyList<string> excludedConnectionIds,
        Func<IChatClient, Task> action,
        CancellationToken ct = default)
        => action(hub.Clients.GroupExcept(chatId.ToString(), excludedConnectionIds));

    public Task SendToUserAsync(Guid userId, Func<IChatClient, Task> action, CancellationToken ct = default)
        => action(hub.Clients.User(userId.ToString()));
}
