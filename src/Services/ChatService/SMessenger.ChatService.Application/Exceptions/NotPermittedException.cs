namespace SMessenger.ChatService.Application.Exceptions;

public class NotPermittedException : Exception
{
    public NotPermittedException(string message) : base(message)
    {
    }
}