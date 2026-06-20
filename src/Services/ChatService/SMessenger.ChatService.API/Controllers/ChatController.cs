using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.Interfaces;

namespace SMessenger.ChatService.API.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatController(IChatService chatService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetChats([FromQuery] string? cursor, CancellationToken ct)
    {
        var result = await chatService.GetUserChatsAsync(UserId, cursor, ct);
        return Ok(result);
    }

    [HttpGet("{chatId:guid}")]
    public async Task<IActionResult> GetChat(Guid chatId, CancellationToken ct)
    {
        var result = await chatService.GetChatAsync(chatId, UserId, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("direct")]
    public async Task<IActionResult> CreateDirectChat([FromBody] CreateDirectChatRequest req, CancellationToken ct)
    {
        var result = await chatService.CreateDirectChatAsync(req, UserId, ct);
        return CreatedAtAction(nameof(GetChat), new { chatId = result.Id }, result);
    }

    [HttpPost("group")]
    public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatRequest req, CancellationToken ct)
    {
        var result = await chatService.CreateGroupChatAsync(req, UserId, ct);
        return CreatedAtAction(nameof(GetChat), new { chatId = result.Id }, result);
    }

    [HttpDelete("{chatId:guid}/leave")]
    public async Task<IActionResult> LeaveChat(Guid chatId, CancellationToken ct)
    {
        await chatService.LeaveChatAsync(chatId, UserId, ct);
        return NoContent();
    }

    [HttpGet("{chatId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid chatId, CancellationToken ct)
    {
        var result = await chatService.GetChatAsync(chatId, UserId, ct);
        if (result is null) return NotFound();
        return Ok(new { items = result.Members });
    }

    [HttpPost("{chatId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid chatId, [FromBody] AddMemberBody body, CancellationToken ct)
    {
        var req = new AddMemberRequest(chatId, body.UserId, body.EncryptedKeyBase64);
        var result = await chatService.AddMemberAsync(req, UserId, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("{chatId:guid}/members/{targetUserId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid chatId, Guid targetUserId, CancellationToken ct)
    {
        await chatService.RemoveMemberAsync(chatId, targetUserId, UserId, ct);
        return NoContent();
    }

    [HttpGet("{chatId:guid}/messages")]
    public async Task<IActionResult> GetMessages(
        Guid chatId,
        [FromQuery] Guid? cursor,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var result = await chatService.GetMessagesAsync(chatId, UserId, cursor, limit, ct);
        return Ok(result);
    }
}

public record AddMemberBody(Guid UserId, string EncryptedKeyBase64);
