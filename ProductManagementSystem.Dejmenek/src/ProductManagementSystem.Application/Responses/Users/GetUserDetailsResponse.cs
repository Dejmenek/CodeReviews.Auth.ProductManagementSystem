namespace ProductManagementSystem.Application.Responses.Users;
public record GetUserDetailsResponse(
    string Id,
    string Email,
    string UserName,
    string? PhoneNumber,
    bool EmailConfirmed,
    List<string> Roles
);
