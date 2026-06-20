using SMessenger.ChatService.Application.DTOs.Pagination;
using SMessenger.ChatService.Application.DTOs.Requests;
using SMessenger.ChatService.Application.DTOs.Responses;
using SMessenger.ChatService.Application.Exceptions;
using SMessenger.ChatService.Application.Interfaces;
using SMessenger.ChatService.Domain.Entities;
using SMessenger.ChatService.Domain.Enums;
using SMessenger.ChatService.Domain.Interfaces;

namespace SMessenger.ChatService.Application.Services;

public class ChatService(
    IChatRepository chatRepo,
    IMessageRepository messageRepo,
    IChatMemberRepository memberRepo,
    IChatEncryptedKeyRepository encryptedKeyRepo,
    IChatNotifier notifier
) : IChatService
{
    private const int DefaultChatPageSize = 20;
    private const int DefaultMessagePageSize = 50;

    public async Task<CursorPage<ChatDto>> GetUserChatsAsync(
        Guid userId, string? cursor, CancellationToken ct = default)
    {
        var userChats = await chatRepo.GetUserChatsAsync(userId, ct);

        var ordered = userChats.ToList();

        var filtered = cursor is null
            ? ordered
            : ordered.SkipWhile(c => c.Id.ToString() != cursor).Skip(1).ToList();

        var pageWithPossibleNext = filtered.Take(DefaultChatPageSize + 1).ToList();

        var hasNextPage = pageWithPossibleNext.Count > DefaultChatPageSize;
        var page = pageWithPossibleNext.Take(DefaultChatPageSize).ToList();

        var nextCursor = hasNextPage ? page.Last().Id.ToString() : null;

        if (page.Count == 0)
        {
            return new CursorPage<ChatDto>(Array.Empty<ChatDto>(), null);
        }

        var chatIds = page.Select(c => c.Id).ToArray();

        var allMembersGrouped = await memberRepo.GetMembersByChatIdsAsync(chatIds, ct);

        var dtos = page.Select(c =>
        {
            var members = allMembersGrouped.Contains(c.Id)
                ? allMembersGrouped[c.Id]
                : Enumerable.Empty<ChatMember>();

            var memberDtos = members
                .Select(x => new ChatMemberDto(x.UserId, x.Role, x.JoinedAt, x.LastReadAt.GetValueOrDefault()))
                .ToArray();

            return new ChatDto(c.Id, c.Type, c.Name, c.CreatedAt, memberDtos);
        }).ToArray();

        return new CursorPage<ChatDto>(dtos, nextCursor);
    }

    public async Task<ChatDto?> GetChatAsync(
        Guid chatId, Guid userId, CancellationToken ct = default)
    {
        await EnsureMemberAsync(chatId, userId, ct);

        var chat = await chatRepo.GetByIdAsync(chatId, ct);
        if (chat is null) return null;

        var chatMembers = await memberRepo.GetMembersAsync(chatId, ct);
        var memberDtos = chatMembers
            .Select(m => new ChatMemberDto(m.UserId, m.Role, m.JoinedAt, m.LastReadAt.GetValueOrDefault()))
            .ToArray();

        return new ChatDto(chat.Id, chat.Type, chat.Name, chat.CreatedAt, memberDtos);
    }

    public async Task<CursorPage<MessageDto>> GetMessagesAsync(
        Guid chatId, Guid userId, Guid? cursor, int limit = 50, CancellationToken ct = default)
    {
        await EnsureMemberAsync(chatId, userId, ct);

        var pageSize = limit is > 0 and <= 200 ? limit : DefaultMessagePageSize;

        var messages = await messageRepo.GetByChatIdAsync(chatId, cursor, pageSize + 1, ct);
        var list = messages.ToList();

        var hasNextPage = list.Count > pageSize;
        var page = list.Take(pageSize).ToList();
        var nextCursor = hasNextPage ? page.Last().Id.ToString() : null;

        var dtos = page.Select(ToMessageDto).ToArray();

        return new CursorPage<MessageDto>(dtos, nextCursor);
    }

    public async Task<ChatDto> CreateDirectChatAsync(
        CreateDirectChatRequest req, Guid callerId, CancellationToken ct = default)
    {
        var existing = await chatRepo.GetDirectChatBetweenAsync(callerId, req.OtherUserId, ct);
        if (existing is not null)
        {
            var existingMembers = await memberRepo.GetMembersAsync(existing.Id, ct);
            var existingMemberDtos = existingMembers
                .Select(m => new ChatMemberDto(m.UserId, m.Role, m.JoinedAt, m.LastReadAt.GetValueOrDefault()))
                .ToArray();

            return new ChatDto(existing.Id, existing.Type, existing.Name, existing.CreatedAt, existingMemberDtos);
        }

        var chat = Chat.Create(ChatType.Direct);
        await chatRepo.CreateAsync(chat, ct);

        var callerMember = ChatMember.Create(chat.Id, callerId, MemberRole.Member);
        var otherMember = ChatMember.Create(chat.Id, req.OtherUserId, MemberRole.Member);
        await memberRepo.CreateAsync(callerMember, ct);
        await memberRepo.CreateAsync(otherMember, ct);

        await encryptedKeyRepo.UpsertAsync(chat.Id, callerId, req.MyEncryptedKeyBase64, ct);
        await encryptedKeyRepo.UpsertAsync(chat.Id, req.OtherUserId, req.OtherUserEncryptedKeyBase64, ct);

        var memberDtos = new[]
        {
            new ChatMemberDto(callerMember.UserId, callerMember.Role, callerMember.JoinedAt, callerMember.LastReadAt.GetValueOrDefault()),
            new ChatMemberDto(otherMember.UserId, otherMember.Role, otherMember.JoinedAt, otherMember.LastReadAt.GetValueOrDefault())
        };

        var dto = new ChatDto(chat.Id, chat.Type, chat.Name, chat.CreatedAt, memberDtos);
        await notifier.SendToUserAsync(req.OtherUserId, client => client.ChatCreated(dto), ct);

        return dto;
    }

    public async Task<ChatDto> CreateGroupChatAsync(
        CreateGroupChatRequest req, Guid callerId, CancellationToken ct = default)
    {
        var chat = Chat.Create(ChatType.Group, req.Name);
        await chatRepo.CreateAsync(chat, ct);

        var membersList = req.Members.ToList();
        var chatMembers = new List<ChatMember>();

        var ownerMember = ChatMember.Create(chat.Id, callerId, MemberRole.Admin);
        await memberRepo.CreateAsync(ownerMember, ct);
        chatMembers.Add(ownerMember);

        foreach (var m in membersList.Where(m => m.UserId != callerId))
        {
            var member = ChatMember.Create(chat.Id, m.UserId, MemberRole.Member);
            await memberRepo.CreateAsync(member, ct);
            chatMembers.Add(member);
        }

        foreach (var m in membersList)
            await encryptedKeyRepo.UpsertAsync(chat.Id, m.UserId, m.EncryptedKeyBase64, ct);

        var memberDtos = chatMembers
            .Select(m => new ChatMemberDto(m.UserId, m.Role, m.JoinedAt, m.LastReadAt.GetValueOrDefault()))
            .ToArray();

        var dto = new ChatDto(chat.Id, chat.Type, chat.Name, chat.CreatedAt, memberDtos);

        foreach (var m in chatMembers.Where(m => m.UserId != callerId))
            await notifier.SendToUserAsync(m.UserId, client => client.ChatCreated(dto), ct);

        return dto;
    }

    public async Task<MessageDto> SendMessageAsync(
        SendMessageRequest req, Guid userId, CancellationToken ct = default)
    {
        await EnsureMemberAsync(req.ChatId, userId, ct);

        var message = Message.Create(req.ChatId, userId, req.CiphertextBase64, req.Nonce);
        await messageRepo.CreateAsync(message, ct);

        var dto = ToMessageDto(message);
        await notifier.SendToGroupAsync(req.ChatId, client => client.ReceiveMessage(dto), ct);

        return dto;
    }

    public async Task<MessageDto> EditMessageAsync(
        EditMessageRequest req, Guid userId, CancellationToken ct = default)
    {
        var message = await messageRepo.GetByIdAsync(req.MessageId, ct)
            ?? throw new MessageNotFoundException(req.MessageId);

        if (message.SenderId != userId)
            throw new NotPermittedException("You can only edit your own messages");

        message.Edit(req.NewCiphertextBase64, req.NewNonce);
        await messageRepo.UpdateAsync(message, ct);

        var dto = ToMessageDto(message);
        await notifier.SendToGroupAsync(message.ChatId, client => client.MessageEdited(dto), ct);

        return dto;
    }

    public async Task DeleteMessageAsync(
        DeleteMessageRequest req, Guid userId, CancellationToken ct = default)
    {
        var message = await messageRepo.GetByIdAsync(req.MessageId, ct)
            ?? throw new MessageNotFoundException(req.MessageId);

        if (message.SenderId != userId)
            throw new NotPermittedException("You can only delete your own messages");

        await messageRepo.SoftDeleteAsync(message, ct);

        await notifier.SendToGroupAsync(
            message.ChatId, client => client.MessageDeleted(req.MessageId, message.ChatId), ct);
    }

    public async Task MarkAsReadAsync(
        MarkAsReadRequest req, Guid userId, CancellationToken ct = default)
    {
        var member = await memberRepo.GetMemberAsync(req.ChatId, userId, ct)
            ?? throw new NotChatMemberException(req.ChatId, userId);

        member.UpdateLastRead(req.ReadAt);
        await memberRepo.UpdateAsync(member, ct);
    }

    public async Task<ChatMemberDto> AddMemberAsync(
        AddMemberRequest req, Guid callerId, CancellationToken ct = default)
    {
        await EnsureRoleAsync(req.ChatId, callerId, ct, MemberRole.Admin);

        if (await memberRepo.IsMemberAsync(req.ChatId, req.UserId, ct))
            throw new NotPermittedException("User is already a member");

        var member = ChatMember.Create(req.ChatId, req.UserId, MemberRole.Member);
        await memberRepo.CreateAsync(member, ct);
        await encryptedKeyRepo.UpsertAsync(req.ChatId, req.UserId, req.EncryptedKeyBase64, ct);

        var dto = new ChatMemberDto(member.UserId, member.Role, member.JoinedAt, member.LastReadAt.GetValueOrDefault());
        await notifier.SendToGroupAsync(req.ChatId, client => client.MemberAdded(req.ChatId, dto), ct);

        return dto;
    }

    public async Task RemoveMemberAsync(
        Guid chatId, Guid targetUserId, Guid callerId, CancellationToken ct = default)
    {
        await EnsureRoleAsync(chatId, callerId, ct, MemberRole.Admin);

        if (!await memberRepo.IsMemberAsync(chatId, targetUserId, ct))
            throw new NotChatMemberException(chatId, targetUserId);

        await memberRepo.RemoveAsync(chatId, targetUserId, ct);
        await encryptedKeyRepo.DeleteByUserAsync(chatId, targetUserId, ct);

        await notifier.SendToGroupAsync(chatId, client => client.MemberRemoved(chatId, targetUserId), ct);
    }

    public async Task LeaveChatAsync(
        Guid chatId, Guid userId, CancellationToken ct = default)
    {
        await EnsureMemberAsync(chatId, userId, ct);

        await memberRepo.RemoveAsync(chatId, userId, ct);
        await encryptedKeyRepo.DeleteByUserAsync(chatId, userId, ct);

        await notifier.SendToGroupAsync(chatId, client => client.MemberRemoved(chatId, userId), ct);
    }

    private async Task EnsureMemberAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        if (!await memberRepo.IsMemberAsync(chatId, userId, ct))
            throw new NotChatMemberException(chatId, userId);
    }

    private async Task EnsureRoleAsync(
        Guid chatId, Guid userId, CancellationToken ct, params MemberRole[] allowedRoles)
    {
        var member = await memberRepo.GetMemberAsync(chatId, userId, ct)
            ?? throw new NotChatMemberException(chatId, userId);

        if (!member.HasRole(allowedRoles))
            throw new NotPermittedException($"Required role: {string.Join(" or ", allowedRoles)}");
    }

    private static MessageDto ToMessageDto(Message m)
        => new(m.Id, m.ChatId, m.SenderId, m.CiphertextBase64, m.Nonce,
               m.IsEdited, m.IsDeleted, m.CreatedAt, m.EditedAt);
}
