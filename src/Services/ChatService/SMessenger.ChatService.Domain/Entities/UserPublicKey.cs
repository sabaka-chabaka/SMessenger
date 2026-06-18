namespace SMessenger.ChatService.Domain.Entities;

public class UserPublicKey
{
    public Guid UserId { get; set; }
    public string PublicKeyBase64 { get; set; } = null!;
    public string Algorithm { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}