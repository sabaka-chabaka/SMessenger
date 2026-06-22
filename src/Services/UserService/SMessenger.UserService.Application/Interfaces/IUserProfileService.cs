using SMessenger.UserService.Application.DTOs.Pagination;
using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.DTOs.Responses;

namespace SMessenger.UserService.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<UserProfileDto> GetMyProfileAsync(Guid userId, CancellationToken ct = default);
    Task<CursorPage<UserSearchResultDto>> SearchUsersAsync(
        SearchUsersRequest req, Guid callerId, CancellationToken ct = default);

    Task<UserProfileDto> CreateProfileAsync(
        Guid userId, string username, string displayName, CancellationToken ct = default);
    Task<UserProfileDto> UpdateProfileAsync(
        UpdateProfileRequest req, Guid callerId, CancellationToken ct = default);
    Task<UserProfileDto> ChangeUsernameAsync(
        ChangeUsernameRequest req, Guid callerId, CancellationToken ct = default);
    Task<UserProfileDto> SetAvatarAsync(
        SetAvatarRequest req, Guid callerId, CancellationToken ct = default);
    Task SetLastSeenVisibilityAsync(
        SetLastSeenVisibilityRequest req, Guid callerId, CancellationToken ct = default);
    Task TouchLastSeenAsync(Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<UserProfileDto>> GetProfilesByIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default);
}