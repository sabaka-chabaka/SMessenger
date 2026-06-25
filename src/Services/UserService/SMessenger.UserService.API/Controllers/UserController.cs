using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.Interfaces;

namespace SMessenger.UserService.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IUserProfileService profileService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var profile = await profileService.GetMyProfileAsync(UserId, ct);
        return Ok(profile);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetProfile(Guid userId, CancellationToken ct)
    {
        var profile = await profileService.GetProfileAsync(userId, ct);
        return Ok(profile);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string q, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var req = new SearchUsersRequest(q, limit);
        var result = await profileService.SearchUsersAsync(req, UserId, ct);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest req, CancellationToken ct)
    {
        var profile = await profileService.UpdateProfileAsync(req, UserId, ct);
        return Ok(profile);
    }

    [HttpPut("me/username")]
    public async Task<IActionResult> ChangeUsername(
        [FromBody] ChangeUsernameRequest req, CancellationToken ct)
    {
        var profile = await profileService.ChangeUsernameAsync(req, UserId, ct);
        return Ok(profile);
    }

    [HttpPut("me/avatar")]
    public async Task<IActionResult> SetAvatar(
        [FromBody] SetAvatarRequest req, CancellationToken ct)
    {
        var profile = await profileService.SetAvatarAsync(req, UserId, ct);
        return Ok(profile);
    }

    [HttpPut("me/last-seen-visibility")]
    public async Task<IActionResult> SetLastSeenVisibility(
        [FromBody] SetLastSeenVisibilityRequest req, CancellationToken ct)
    {
        await profileService.SetLastSeenVisibilityAsync(req, UserId, ct);
        return NoContent();
    }

    [HttpPost("batch")]
    public async Task<IActionResult> GetProfilesByIds(
        [FromBody] IReadOnlyList<Guid> userIds, CancellationToken ct)
    {
        var profiles = await profileService.GetProfilesByIdsAsync(userIds, ct);
        return Ok(new { items = profiles });
    }
}