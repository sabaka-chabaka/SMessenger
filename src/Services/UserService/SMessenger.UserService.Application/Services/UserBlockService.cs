using SMessenger.UserService.Application.DTOs.Requests;
using SMessenger.UserService.Application.DTOs.Responses;
using SMessenger.UserService.Application.Exceptions;
using SMessenger.UserService.Application.Interfaces;
using SMessenger.UserService.Domain.Entities;
using SMessenger.UserService.Domain.Repositories;

namespace SMessenger.UserService.Application.Services;

public class UserBlockService(IUserBlockRepository blocks) : IUserBlockService
{
    public async Task<IReadOnlyList<UserBlockDto>> GetBlocklistAsync(Guid callerId, CancellationToken ct = default)
    {
        var entities = await blocks.GetBlocksByUserAsync(callerId, ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<bool> IsBlockedAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct = default)
    {
        return await blocks.IsBlockedAsync(blockerUserId, blockedUserId, ct);
    }

    public async Task BlockUserAsync(BlockUserRequest req, Guid callerId, CancellationToken ct = default)
    {
        UserBlock block;
        try
        {
            block = UserBlock.Create(callerId, req.TargetUserId);
        }
        catch (InvalidOperationException)
        {
            throw new CannotBlockSelfException();
        }
        
        await blocks.CreateAsync(block, ct);
    }

    public async Task UnblockUserAsync(Guid targetUserId, Guid callerId, CancellationToken ct = default)
    {
        await blocks.DeleteAsync(callerId, targetUserId, ct);
    }

    private UserBlockDto MapToDto(UserBlock input)
    {
        return new UserBlockDto(input.BlockedUserId, input.CreatedAt);
    }
}