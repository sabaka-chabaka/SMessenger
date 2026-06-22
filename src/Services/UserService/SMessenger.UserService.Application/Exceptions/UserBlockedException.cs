namespace SMessenger.UserService.Application.Exceptions;

public class UserBlockedException : Exception
{
    public UserBlockedException(Guid userId)
        : base($"Action not permitted: user {userId} has been blocked.")
    {
    }
}