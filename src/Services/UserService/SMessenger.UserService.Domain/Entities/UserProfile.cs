namespace SMessenger.UserService.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public DateTime? LastSeenAt { get; private set; }
    public bool ShowLastSeen { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    private UserProfile() { }
    
    public static UserProfile Create(Guid userId, string username, string displayName)
    {
        return new UserProfile
        {
            UserId = userId,
            Username = username.ToLowerInvariant(),
            DisplayName = displayName,
            ShowLastSeen = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string displayName, string? bio)
    {
        DisplayName = displayName;
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeUsername(string newUsername)
    {
        Username = newUsername.ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void TouchLastSeen(DateTime seenAt)
        => LastSeenAt = seenAt;

    public void SetLastSeenVisibility(bool visible)
    {
        ShowLastSeen = visible;
        UpdatedAt = DateTime.UtcNow;
    }
}