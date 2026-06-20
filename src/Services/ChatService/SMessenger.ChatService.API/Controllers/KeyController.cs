using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.Interfaces;

namespace SMessenger.ChatService.API.Controllers;

[ApiController]
[Route("api/keys")]
[Authorize]
public class KeyController(IKeyDistributionService keyService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("public/{userId:guid}")]
    public async Task<IActionResult> GetPublicKey(Guid userId, CancellationToken ct)
    {
        var key = await keyService.GetPublicKeyAsync(userId, ct);
        return Ok(new { key.PublicKeyBase64, key.Algorithm });
    }

    [HttpPost("public")]
    public async Task<IActionResult> UploadPublicKey([FromBody] UploadPublicKeyRequest req, CancellationToken ct)
    {
        await keyService.StorePublicKeyAsync(UserId, req.PublicKeyBase64, req.Algorithm, ct);
        return NoContent();
    }

    [HttpGet("chat/{chatId:guid}")]
    public async Task<IActionResult> GetEncryptedChatKey(Guid chatId, CancellationToken ct)
    {
        var key = await keyService.GetEncryptedChatKeyAsync(chatId, UserId, ct);
        return Ok(new { key.EncryptedKeyBase64 });
    }

    [HttpPost("chat/{chatId:guid}")]
    public async Task<IActionResult> UploadEncryptedChatKeys(
        Guid chatId,
        [FromBody] UploadEncryptedKeysRequest req,
        CancellationToken ct)
    {
        var entries = req.EncryptedKeys
            .Select(k => (k.UserId, k.EncryptedKeyBase64))
            .ToList();

        await keyService.StoreEncryptedChatKeysAsync(chatId, UserId, entries, ct);
        return NoContent();
    }
}
