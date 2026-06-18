namespace SMessenger.ChatService.Application.DTOs.Requests;

public record MarkAsReadRequest(
    Guid ChatId,
    DateTime ReadAt
);