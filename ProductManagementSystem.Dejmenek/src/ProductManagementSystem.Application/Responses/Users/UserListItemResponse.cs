namespace ProductManagementSystem.Application.Responses.Users;
public record UserListItemResponse(
    string Id,
    string Email,
    string UserName,
    string? PhoneNumber,
    bool EmailConfirmed,
    List<string> Roles);
