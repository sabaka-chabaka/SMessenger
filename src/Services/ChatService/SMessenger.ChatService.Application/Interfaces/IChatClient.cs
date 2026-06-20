using SMessenger.ChatService.Application.DTOs.Responses;

namespace SMessenger.ChatService.Application.Interfaces;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task MessageEdited(MessageDto message);
    Task MessageDeleted(Guid messageId, Guid chatId);
    Task UserTyping(Guid chatId, Guid userId);
    Task UserStoppedTyping(Guid chatId, Guid userId);
    Task MemberAdded(Guid chatId, ChatMemberDto member);
    Task MemberRemoved(Guid chatId, Guid userId);
    Task UserOnline(Guid userId);
    Task UserOffline(Guid userId);
    Task ChatCreated(ChatDto chat);
}