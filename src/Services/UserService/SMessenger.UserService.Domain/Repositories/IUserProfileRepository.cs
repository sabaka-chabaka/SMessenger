using SMessenger.UserService.Domain.Entities;

namespace SMessenger.UserService.Domain.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<UserProfile?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<IReadOnlyList<UserProfile>> SearchByUsernameAsync(
        string query, int limit = 20, CancellationToken ct = default);
    Task<IReadOnlyList<UserProfile>> GetByUserIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default);
    Task<bool> IsUsernameTakenAsync(string username, CancellationToken ct = default);
    Task CreateAsync(UserProfile profile, CancellationToken ct = default);
    Task UpdateAsync(UserProfile profile, CancellationToken ct = default);
}