namespace SMessenger.ChatService.Application.DTOs.Requests;

public record DeleteMessageRequest(
    Guid MessageId
);