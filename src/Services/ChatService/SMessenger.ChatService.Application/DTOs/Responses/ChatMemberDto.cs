using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Application.DTOs.Responses;

public record ChatMemberDto(
    Guid UserId,
    MemberRole Role,
    DateTime JoinedAt,
    DateTime LastReadAt
);