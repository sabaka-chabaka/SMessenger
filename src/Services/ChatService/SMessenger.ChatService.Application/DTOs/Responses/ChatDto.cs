using SMessenger.ChatService.Domain.Enums;

namespace SMessenger.ChatService.Application.DTOs.Responses;

public record ChatDto(
    Guid Id,
    ChatType Type,
    string Name,
    DateTime CreatedAt
);