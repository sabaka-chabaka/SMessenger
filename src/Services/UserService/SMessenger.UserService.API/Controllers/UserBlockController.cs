using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.Interfaces;

namespace SMessenger.UserService.API.Controllers;

[ApiController]
[Route("api/users/me/blocklist")]
[Authorize]
public class UserBlockController(IUserBlockService blockService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetBlocklist(CancellationToken ct)
    {
        var blocks = await blockService.GetBlocklistAsync(UserId, ct);
        return Ok(new { items = blocks });
    }

    [HttpPost]
    public async Task<IActionResult> BlockUser(
        [FromBody] BlockUserRequest req, CancellationToken ct)
    {
        await blockService.BlockUserAsync(req, UserId, ct);
        return NoContent();
    }

    [HttpDelete("{targetUserId:guid}")]
    public async Task<IActionResult> UnblockUser(Guid targetUserId, CancellationToken ct)
    {
        await blockService.UnblockUserAsync(targetUserId, UserId, ct);
        return NoContent();
    }
}