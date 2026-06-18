using SMessenger.ChatService.Application.DTOs.Pagination;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.DTOs.Responses;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IChatService
{
    Task<CursorPage<ChatDto>> GetUserChatsAsync(Guid userId, string? cursor, CancellationToken ct = default);
    Task<ChatDto?> GetChatAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task<ChatDto> CreateDirectChatAsync(CreateDirectChatRequest req, CancellationToken ct = default);
    Task<ChatDto> CreateGroupChatAsync(CreateGroupChatRequest req, CancellationToken ct = default);
    Task<MessageDto> SendMessageAsync(SendMessageRequest req, Guid userId, CancellationToken ct = default);
    Task<MessageDto> EditMessageAsync(EditMessageRequest req, Guid userId, CancellationToken ct = default);
    Task DeleteMessageAsync(DeleteMessageRequest req, Guid userId, CancellationToken ct = default);
    Task<MessageDto> MarkAsReadAsync(MarkAsReadRequest req, Guid userId, CancellationToken ct = default);
    Task<ChatMemberDto> AddMemberAsync(AddMemberRequest req, Guid userId, CancellationToken ct = default);
    Task RemoveMemberAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    Task LeaveChatAsync(Guid chatId, Guid userId, CancellationToken ct = default);
}