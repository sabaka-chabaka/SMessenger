namespace SMessenger.ChatService.Application.Exceptions;

public class NotChatMemberException : Exception
{
    public NotChatMemberException(Guid chatId, Guid userId)
        : base($"User {userId} is not a member of chat {chatId}.")
    {
    }
}