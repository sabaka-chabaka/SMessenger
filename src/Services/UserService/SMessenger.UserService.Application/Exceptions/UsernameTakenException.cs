namespace SMessenger.UserService.Application.Exceptions;

public class UsernameTakenException : Exception
{
    public UsernameTakenException(string username)
        : base($"Username '{username}' is already taken.")
    {
    }
}