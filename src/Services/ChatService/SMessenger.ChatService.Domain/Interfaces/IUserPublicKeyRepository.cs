namespace SMessenger.ChatService.Domain.Interfaces;

public interface IUserPublicKeyRepository
{
    Task<string?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task UpsertAsync(Guid userId, string publicKey, CancellationToken ct = default);
}