namespace SMessenger.ChatService.Application.Exceptions;

public class MessageNotFoundException : Exception
{
    public MessageNotFoundException(Guid messageId)
        : base($"Message with ID {messageId} was not found.")
    {
    }
}
