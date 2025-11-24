namespace ProductManagementSystem.Application.Requests.Users;
public record CreateUserRequest(string UserName, string Email, string? PhoneNumber, string Password);
