namespace ProductManagementSystem.Application.Responses.Users;
public record GetUserForUpdateResponse(
    string Id,
    string Email,
    string UserName,
    string? PhoneNumber
);
