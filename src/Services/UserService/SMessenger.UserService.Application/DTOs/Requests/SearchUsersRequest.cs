namespace SMessenger.UserService.Application.DTOs.Requests;

public record SearchUsersRequest(
    string Query,
    int Limit = 20
);