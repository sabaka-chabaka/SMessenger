using SMessenger.UserService.Application.DTOs.Pagination;
using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.DTOs.Responses;
using SMessenger.UserService.Application.Exceptions;
using SMessenger.UserService.Application.Interfaces;
using SMessenger.UserService.Domain.Entities;
using SMessenger.UserService.Domain.Repositories;

namespace SMessenger.UserService.Application.Services;

public class UserProfileService(IUserProfileRepository profiles) : IUserProfileService
{
    public async Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await profiles.GetByUserIdAsync(userId, ct);
        if (profile == null) throw new UserNotFoundException(userId);
        return MapToDto(profile);
    }

    public async Task<UserProfileDto> GetMyProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await profiles.GetByUserIdAsync(userId, ct);
        if (profile == null) throw new UserNotFoundException(userId);
        return MapToDto(profile);
    }

    public async Task<CursorPage<UserSearchResultDto>> SearchUsersAsync(
        SearchUsersRequest req, 
        Guid callerId, 
        CancellationToken ct = default)
    {
        var users = await profiles.SearchByUsernameAsync(req.Query, req.Limit, ct);
        
        var dtos = users
            .Where(u => u.UserId != callerId)
            .Select(u => new UserSearchResultDto(
                u.UserId,
                u.Username,
                u.DisplayName,
                u.AvatarUrl))
            .ToList();

        return new CursorPage<UserSearchResultDto>(dtos, null);
    }


    public async Task<UserProfileDto> CreateProfileAsync(Guid userId, string username, string displayName, CancellationToken ct = default)
    {
        if (await profiles.IsUsernameTakenAsync(username, ct)) throw new UsernameTakenException(username);
        var user = UserProfile.Create(userId, username, displayName);
        await profiles.CreateAsync(user, ct);
        return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(UpdateProfileRequest req, Guid callerId, CancellationToken ct = default)
    {
        var user = await profiles.GetByUserIdAsync(callerId, ct);
        if (user == null) throw new UserNotFoundException(callerId);
        user.UpdateProfile(req.DisplayName, req.Bio);
        await profiles.UpdateAsync(user, ct);
        return MapToDto(user);
    }

    public async Task<UserProfileDto> ChangeUsernameAsync(ChangeUsernameRequest req, Guid callerId, CancellationToken ct = default)
    {
        var user = await profiles.GetByUserIdAsync(callerId, ct);
        if (user == null) throw new UserNotFoundException(callerId);
        if (await profiles.IsUsernameTakenAsync(req.NewUsername, ct)) throw new UsernameTakenException(req.NewUsername);
        user.ChangeUsername(req.NewUsername);
        await profiles.UpdateAsync(user, ct);
        return MapToDto(user);
    }

    public async Task<UserProfileDto> SetAvatarAsync(SetAvatarRequest req, Guid callerId, CancellationToken ct = default)
    {
        var user = await profiles.GetByUserIdAsync(callerId, ct);
        if (user == null) throw new UserNotFoundException(callerId);
        user.SetAvatar(req.AvatarUrl);
        await profiles.UpdateAsync(user, ct);
        return MapToDto(user);
    }

    public async Task SetLastSeenVisibilityAsync(SetLastSeenVisibilityRequest req, Guid callerId, CancellationToken ct = default)
    {
        var user = await profiles.GetByUserIdAsync(callerId, ct);
        if (user == null) throw new UserNotFoundException(callerId);
        user.SetLastSeenVisibility(req.Visible);
        await profiles.UpdateAsync(user, ct);
    }

    public async Task TouchLastSeenAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await profiles.GetByUserIdAsync(userId, ct);
        if (user == null) throw new UserNotFoundException(userId);
        user.TouchLastSeen(DateTime.UtcNow);
        await profiles.UpdateAsync(user, ct);
    }

    public async Task<IReadOnlyList<UserProfileDto>> GetProfilesByIdsAsync(IReadOnlyList<Guid> userIds, CancellationToken ct = default)
    {
        var profiless = await profiles.GetByUserIdsAsync(userIds, ct);
        if (profiless == null) throw new UserNotFoundException(userIds.FirstOrDefault());
        return profiless.Select(MapToDto).ToList();
    }

    private UserProfileDto MapToDto(UserProfile profile)
    {
        return new UserProfileDto(profile.UserId, profile.Username, profile.DisplayName, profile.AvatarUrl, profile.Bio, profile.LastSeenAt, profile.ShowLastSeen);
    }
}