namespace ProductManagementSystem.Application.Requests.Users;
public record UpdateUserRequest(
    string Id,
    string UserName,
    string Email,
    string? PhoneNumber
);
