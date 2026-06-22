namespace SMessenger.UserService.Application.Exceptions;

public class CannotBlockSelfException : Exception
{
    public CannotBlockSelfException()
        : base("A user cannot block themselves.")
    {
    }
}