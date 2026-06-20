using SMessenger.ChatService.Domain.Entities;

namespace SMessenger.ChatService.Domain.Interfaces;

public interface IUserPublicKeyRepository
{
    Task<UserPublicKey?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserPublicKey>> GetByUserIdsAsync(IReadOnlyList<Guid> userIds, CancellationToken ct = default);
    Task UpsertAsync(Guid userId, string publicKey, string algorithm, CancellationToken ct = default);
}