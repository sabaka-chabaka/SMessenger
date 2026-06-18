using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.DTOs.Responses;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IChatService
{
    public Task<IReadOnlyCollection<Chat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default);
    public Task<Chat?> GetChatAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    public Task<ChatDto> CreateDirectChatAsync(CreateDirectChatRequest req, CancellationToken ct = default);
    public Task<ChatDto> CreateGroupChatAsync(CreateGroupChatRequest req, CancellationToken ct = default);
    public Task<MessageDto> SendMessageAsync(SendMessageRequest req, Guid userId, CancellationToken ct = default);
    public Task<MessageDto> EditMessageAsync(EditMessageRequest req, Guid userId, CancellationToken ct = default);
    public Task DeleteMessageAsync(DeleteMessageRequest req, Guid userId, CancellationToken ct = default);
    public Task<MessageDto> MarkAsReadAsync(MarkAsReadRequest req, Guid userId, CancellationToken ct = default);
    public Task<ChatMemberDto> AddMemberAsync(AddMemberRequest req, Guid userId, CancellationToken ct = default);
    public Task RemoveMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    public Task LeaveChatAsync(Guid chatId, Guid userId, CancellationToken ct = default);
}