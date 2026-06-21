using SMessenger.UserService.Domain.Entities;

namespace SMessenger.UserService.Domain.Repositories;

public interface IUserBlockRepository
{
    Task<IReadOnlyList<UserBlock>> GetBlocksByUserAsync(Guid blockerUserId, CancellationToken ct = default);
    Task<bool> IsBlockedAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default);
    Task CreateAsync(UserBlock block, CancellationToken ct = default);
    Task DeleteAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default);
}