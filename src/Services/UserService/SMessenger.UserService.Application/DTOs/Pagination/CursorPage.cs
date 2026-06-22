namespace SMessenger.UserService.Application.DTOs.Pagination;

public record CursorPage<T>(
    IEnumerable<T> Items,
    string? NextCursor
);