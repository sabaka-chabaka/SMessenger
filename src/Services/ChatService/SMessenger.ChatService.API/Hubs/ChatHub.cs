using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.Interfaces;

namespace SMessenger.ChatService.API.Hubs;

[Authorize]
public class ChatHub(
    IChatService chatService,
    IPresenceService presenceService
) : Hub<IChatClient>
{
    private Guid UserId => Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private Guid ConnId => DeriveConnId(Context.ConnectionId);

    public override async Task OnConnectedAsync()
    {
        var userId = UserId;

        await presenceService.UserConnectedAsync(userId, ConnId);

        // Join all of the user's chat groups so server-side group broadcasts reach this connection
        var chats = await chatService.GetUserChatsAsync(userId, null);
        var chatIds = chats.Items.Select(c => c.Id).ToArray();

        foreach (var chatId in chatIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        // Notify only the chats this user belongs to, not every connection on the server
        foreach (var chatId in chatIds)
            await Clients.OthersInGroup(chatId.ToString()).UserOnline(userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = UserId;

        await presenceService.UserDisconnectedAsync(userId, ConnId);

        var stillOnline = await presenceService.IsOnlineAsync(userId);
        if (!stillOnline)
        {
            var chats = await chatService.GetUserChatsAsync(userId, null);
            foreach (var chat in chats.Items)
                await Clients.OthersInGroup(chat.Id.ToString()).UserOffline(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid chatId, string ciphertextBase64, string nonce)
    {
        var req = new SendMessageRequest(chatId, ciphertextBase64, nonce);
        await chatService.SendMessageAsync(req, UserId);
    }

    public async Task EditMessage(Guid messageId, string ciphertextBase64, string nonce)
    {
        var req = new EditMessageRequest(messageId, ciphertextBase64, nonce);
        await chatService.EditMessageAsync(req, UserId);
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var req = new DeleteMessageRequest(messageId);
        await chatService.DeleteMessageAsync(req, UserId);
    }

    public async Task MarkAsRead(Guid chatId)
    {
        var req = new MarkAsReadRequest(chatId, DateTime.UtcNow);
        await chatService.MarkAsReadAsync(req, UserId);
    }

    public async Task StartTyping(Guid chatId)
    {
        await Clients.OthersInGroup(chatId.ToString()).UserTyping(chatId, UserId);
    }

    public async Task StopTyping(Guid chatId)
    {
        await Clients.OthersInGroup(chatId.ToString()).UserStoppedTyping(chatId, UserId);
    }

    /// <summary>
    /// SignalR connection IDs are opaque strings, not GUIDs. The presence cache is keyed
    /// by Guid, so we deterministically derive one from the connection ID (stable for the
    /// lifetime of the connection, unique enough to avoid collisions in practice).
    /// </summary>
    private static Guid DeriveConnId(string connectionId)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(connectionId));
        return new Guid(hash);
    }
}
