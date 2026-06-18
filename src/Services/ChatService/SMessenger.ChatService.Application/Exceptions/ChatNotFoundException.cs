namespace SMessenger.ChatService.Application.Exceptions;

public class ChatNotFoundException : Exception
{
    public ChatNotFoundException(Guid chatId) 
        : base($"Chat with ID {chatId} was not found.")
    {
    }
}