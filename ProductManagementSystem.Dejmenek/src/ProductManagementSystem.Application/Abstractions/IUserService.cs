using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Abstractions;
public interface IUserService
{
    Task<Result<Paged<UserListItemResponse>>> GetUsersAsync(GetUsersRequest filter);
    Task<Result<GetUserDetailsResponse>> GetDetailsAsync(string userId);
    Task<Result<GetUserForUpdateResponse>> GetUserForUpdateAsync(string userId);
    Task<Result> RemoveSingleUserAsync(string userId);
    Task<Result> RemoveUsersAsync(List<string> userIds);
    Task<Result> UpdateUserAsync(UpdateUserRequest request);
    Task<Result<IEnumerable<RoleResponse>>> GetUserRolesForUpdateAsync(string userId);
    Task<Result> UpdateUserRolesAsync(string userId, List<string> selectedRoles);
    Task<Result> CreateUserAsync(CreateUserRequest request);
}
