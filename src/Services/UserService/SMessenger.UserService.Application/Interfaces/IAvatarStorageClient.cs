namespace SMessenger.UserService.Application.Interfaces;

public interface IAvatarStorageClient
{
    Task<string> UploadAvatarAsync(Guid userId, Stream content, string contentType, CancellationToken ct = default);
    Task DeleteAvatarAsync(string avatarUrl, CancellationToken ct = default);
}