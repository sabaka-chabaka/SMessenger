namespace SMessenger.UserService.Domain.Entities;

public class UserBlock
{
    public Guid BlockerUserId { get; private set; }
    public Guid BlockedUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private UserBlock() { }
    
    public static UserBlock Create(Guid blockerUserId, Guid blockedUserId)
    {
        if (blockerUserId == blockedUserId)
            throw new InvalidOperationException("A user cannot block themselves.");

        return new UserBlock
        {
            BlockerUserId = blockerUserId,
            BlockedUserId = blockedUserId,
            CreatedAt = DateTime.UtcNow
        };
    }
}