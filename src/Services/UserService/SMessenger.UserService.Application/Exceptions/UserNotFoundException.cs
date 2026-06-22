namespace SMessenger.UserService.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId)
        : base($"User profile with ID {userId} was not found.")
    {
    }
}