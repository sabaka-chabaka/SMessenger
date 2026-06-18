namespace SMessenger.ChatService.Application.DTOs.Requests;

public record UploadPublicKeyRequest(
    string PublicKeyBase64,
    string Algorithm
);