using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Exceptions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetUsersRequest> _getUsersRequestValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserRequestValidator;
    private readonly IValidator<CreateUserRequest> _createUserRequestValidator;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IUserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IValidator<GetUsersRequest> getUsersRequestValidator,
        IValidator<UpdateUserRequest> updateUserRequestValidator,
        IValidator<CreateUserRequest> createUserRequestValidator,
        IMemoryCache cache,
        ILogger<IUserService> logger)
    {
        _userRepository = userRepository;
        _getUsersRequestValidator = getUsersRequestValidator;
        _updateUserRequestValidator = updateUserRequestValidator;
        _createUserRequestValidator = createUserRequestValidator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<Paged<UserListItemResponse>>> GetUsersAsync(GetUsersRequest filter)
    {
        _logger.LogInformation("Getting users with filter: {@Filter}", filter);

        var validationResult = await _getUsersRequestValidator.ValidateAsync(filter);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetUsersRequest: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());

            return Result.Failure<Paged<UserListItemResponse>>(error);
        }

        try
        {
            var users = await _userRepository.GetUsersAsync(
                filter.Search,
                filter.EmailConfirmed,
                filter.PageNumber,
                filter.PageSize
            );

            _logger.LogInformation("Successfully retrieved {Count} users for page {PageNumber}", users.Items.Count, filter.PageNumber);
            return Result.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users with filter: {@Filter}", filter);

            return Result.Failure<Paged<UserListItemResponse>>(UserErrors.GetUsersFailed);
        }
    }

    public async Task<Result<GetUserDetailsResponse>> GetDetailsAsync(string userId)
    {
        _logger.LogInformation("Getting details for user: {UserId}", userId);

        string cacheKey = $"UserDetails_{userId}";
        if (_cache.TryGetValue(cacheKey, out GetUserDetailsResponse? cachedUser))
        {
            _logger.LogInformation("User details retrieved from cache for user: {UserId}", userId);
            return Result.Success(cachedUser!);
        }

        try
        {
            var user = await _userRepository.GetDetailsAsync(userId);
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));

            _logger.LogInformation("Successfully retrieved details for user: {UserId}", userId);
            return Result.Success(user);
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found exception for user: {UserId}", userId);
            return Result.Failure<GetUserDetailsResponse>(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving details for user: {UserId}", userId);
            return Result.Failure<GetUserDetailsResponse>(UserErrors.GetDetailsAsyncFailed);
        }
    }

    public async Task<Result> RemoveSingleUserAsync(string userId)
    {
        _logger.LogInformation("Attempting to remove user: {UserId}", userId);

        try
        {
            var isAdmin = await _userRepository.IsUserInRoleAsync(userId, "Admin");

            if (isAdmin)
            {
                _logger.LogWarning("Attempted to delete admin user: {UserId}", userId);
                return Result.Failure(UserErrors.AdminUserDeletionNotAllowed);
            }

            await _userRepository.RemoveSingleUserAsync(userId);
            _logger.LogInformation("Successfully removed user: {UserId}", userId);

            _cache.Remove($"UserDetails_{userId}");
            return Result.Success();
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found for removal: {UserId}", userId);
            return Result.Failure(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user: {UserId}", userId);
            return Result.Failure(UserErrors.RemoveSingleUserFailed);
        }
    }

    public async Task<Result> RemoveUsersAsync(List<string> userIds)
    {
        _logger.LogInformation("Attempting to remove users: {UserIds}", userIds);

        try
        {
            foreach (var userId in userIds)
            {
                var isAdmin = await _userRepository.IsUserInRoleAsync(userId, "Admin");

                if (isAdmin)
                {
                    _logger.LogWarning("Attempted to delete admin user in bulk operation: {UserId}", userId);
                    return Result.Failure(UserErrors.AdminUserDeletionNotAllowed);
                }
            }

            await _userRepository.RemoveUsersAsync(userIds);
            _logger.LogInformation("Successfully removed users: {UserIds}", userIds);

            foreach (var userId in userIds)
            {
                _cache.Remove($"UserDetails_{userId}");
            }

            return Result.Success();
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("One or more users not found for removal: {UserIds}", userIds);
            return Result.Failure(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing users: {UserIds}", userIds);
            return Result.Failure(UserErrors.RemoveUsersFailed);
        }
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request)
    {
        _logger.LogInformation("Attempting to update user: {UserId}", request.Id);

        var validationResult = _updateUserRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateUserRequest: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());
            return Result.Failure(error);
        }

        try
        {
            var isAdmin = await _userRepository.IsUserInRoleAsync(request.Id, "Admin");

            if (isAdmin)
            {
                _logger.LogWarning("Attempted to update admin user: {UserId}", request.Id);
                return Result.Failure(UserErrors.AdminUserUpdateNotAllowed);
            }

            await _userRepository.UpdateUserAsync(request);

            _logger.LogInformation("Successfully updated user: {UserId}", request.Id);

            _cache.Remove($"UserDetails_{request.Id}");
            return Result.Success();
        }
        catch (DuplicateUserNameException)
        {
            _logger.LogWarning("Duplicate username for update: {UserName}", request.UserName);
            return Result.Failure(UserErrors.DuplicateUserName);
        }
        catch (DuplicateEmailException)
        {
            _logger.LogWarning("Duplicate email for update: {Email}", request.Email);
            return Result.Failure(UserErrors.DuplicateEmail);
        }
        catch (DuplicatePhoneNumberException)
        {
            _logger.LogWarning("Duplicate phone number for update: {PhoneNumber}", request.PhoneNumber);
            return Result.Failure(UserErrors.DuplicatePhoneNumber);
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found for update: {UserId}", request.Id);
            return Result.Failure(UserErrors.UserNotFound);
        }
        catch (UserUpdateException ex)
        {
            _logger.LogError("User update failed: {Message}", ex.Message);
            return Result.Failure(UserErrors.UserUpdateFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user: {UserId}", request.Id);
            return Result.Failure(UserErrors.UserUpdateError);
        }
    }

    public async Task<Result<GetUserForUpdateResponse>> GetUserForUpdateAsync(string userId)
    {
        _logger.LogInformation("Getting user for update: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetUserForUpdateAsync(userId);

            _logger.LogInformation("Successfully retrieved user for update: {UserId}", userId);
            return Result.Success(user);
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found for update: {UserId}", userId);
            return Result.Failure<GetUserForUpdateResponse>(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user for update: {UserId}", userId);
            return Result.Failure<GetUserForUpdateResponse>(UserErrors.GetUserForUpdateError);
        }
    }

    public async Task<Result<IEnumerable<RoleResponse>>> GetUserRolesForUpdateAsync(string userId)
    {
        _logger.LogInformation("Getting user roles for update: {UserId}", userId);

        try
        {
            var assignedRoles = await _userRepository.GetUserRolesAsync(userId);
            var allRoles = await _userRepository.GetAllRolesAsync();

            _logger.LogInformation("Successfully retrieved roles for user: {UserId}", userId);

            return Result.Success(allRoles.Select(r => new RoleResponse(
                r.Id, r.Name!, assignedRoles.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            ));
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found when getting roles for update: {UserId}", userId);
            return Result.Failure<IEnumerable<RoleResponse>>(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user roles for update: {UserId}", userId);
            return Result.Failure<IEnumerable<RoleResponse>>(UserErrors.GetUserRolesForUpdateFailed);
        }
    }

    public async Task<Result> UpdateUserRolesAsync(string userId, List<string> selectedRoles)
    {
        _logger.LogInformation("Updating roles for user: {UserId} with roles: {@SelectedRoles}", userId, selectedRoles);

        try
        {
            var isAdmin = await _userRepository.IsUserInRoleAsync(userId, "Admin");

            if (isAdmin)
            {
                _logger.LogWarning("Attempted to update admin user: {UserId}", userId);
                return Result.Failure(UserErrors.AdminUserUpdateNotAllowed);
            }

            await _userRepository.UpdateUserRolesAsync(userId, selectedRoles);
            _logger.LogInformation("Successfully updated roles for user: {UserId}", userId);

            _cache.Remove($"UserDetails_{userId}");
            return Result.Success();
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("User not found when updating roles: {UserId}", userId);
            return Result.Failure(UserErrors.UserNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user roles: {UserId}", userId);
            return Result.Failure(UserErrors.UpdateUserRolesError);
        }
    }

    public async Task<Result> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation("Attempting to create user: {UserName}", request.UserName);

        var validationResult = await _createUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateUserRequest: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());

            return Result.Failure(error);
        }

        try
        {
            await _userRepository.CreateUserAsync(request);
            _logger.LogInformation("Successfully created user: {UserName}", request.UserName);
            return Result.Success();
        }
        catch (DuplicateUserNameException)
        {
            _logger.LogWarning("Duplicate username for creation: {UserName}", request.UserName);
            return Result.Failure(UserErrors.DuplicateUserName);
        }
        catch (DuplicateEmailException)
        {
            _logger.LogWarning("Duplicate email for creation: {Email}", request.Email);
            return Result.Failure(UserErrors.DuplicateEmail);
        }
        catch (DuplicatePhoneNumberException)
        {
            _logger.LogWarning("Duplicate phone number for creation: {PhoneNumber}", request.PhoneNumber);
            return Result.Failure(UserErrors.DuplicatePhoneNumber);
        }
        catch (UserCreationException ex)
        {
            _logger.LogError("User creation failed: {Message}", ex.Message);
            return Result.Failure(UserErrors.CreateUserFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user: {UserName}", request.UserName);
            return Result.Failure(UserErrors.CreateUserError);
        }
    }
}
