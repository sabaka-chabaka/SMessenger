using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.DTOs.Responses;

namespace SMessenger.UserService.Application.Interfaces;

public interface IUserBlockService
{
    Task<IReadOnlyList<UserBlockDto>> GetBlocklistAsync(Guid callerId, CancellationToken ct = default);
    Task<bool> IsBlockedAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default);
    Task BlockUserAsync(BlockUserRequest req, Guid callerId, CancellationToken ct = default);
    Task UnblockUserAsync(Guid targetUserId, Guid callerId, CancellationToken ct = default);
}