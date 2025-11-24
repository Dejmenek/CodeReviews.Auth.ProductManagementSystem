using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Abstractions;
public interface IUserRepository
{
    Task<Paged<UserListItemResponse>> GetUsersAsync(string? search,
        bool? emailConfirmed,
        int pageNumber,
        PageSize pageSize);

    Task<GetUserDetailsResponse> GetDetailsAsync(string userId);
    Task<GetUserForUpdateResponse> GetUserForUpdateAsync(string userId);

    Task RemoveSingleUserAsync(string userId);
    Task RemoveUsersAsync(List<string> userIds);

    Task<bool> UserExists(string userId);

    Task<bool> IsUserInRoleAsync(string userId, string role);
    Task<bool> UpdateUserAsync(UpdateUserRequest request);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task UpdateUserRolesAsync(string userId, List<string> selectedRoles);
    Task CreateUserAsync(CreateUserRequest request);
}
