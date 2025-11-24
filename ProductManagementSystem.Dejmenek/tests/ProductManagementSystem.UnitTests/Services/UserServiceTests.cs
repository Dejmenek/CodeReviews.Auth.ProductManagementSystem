using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Exceptions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Application.Services;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.UnitTests.Services;
public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetUsersRequest> _getUsersRequestValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserRequestValidator;
    private readonly IValidator<CreateUserRequest> _createUserRequestValidator;
    private readonly ILogger<IUserService> _logger;
    private readonly IMemoryCache _cache;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _getUsersRequestValidator = Substitute.For<IValidator<GetUsersRequest>>();
        _updateUserRequestValidator = Substitute.For<IValidator<UpdateUserRequest>>();
        _createUserRequestValidator = Substitute.For<IValidator<CreateUserRequest>>();
        _cache = Substitute.For<IMemoryCache>();
        _logger = Substitute.For<ILogger<IUserService>>();
        _userService = new UserService(
            _userRepository,
            _getUsersRequestValidator,
            _updateUserRequestValidator,
            _createUserRequestValidator,
            _cache,
            _logger
        );
    }

    private static CreateUserRequest ValidCreateUserRequest() =>
        new("user1", "user1@test.com", "+1234567890", "e@mple123!");

    private static RoleResponse ValidRoleResponse(string id, string name, bool assigned) =>
        new(id, name, assigned);

    private static List<string> ValidRoles() => new() { "Admin", "User" };

    private static GetUsersRequest ValidGetUsersRequest() =>
    new("search", true, 1, PageSize.Ten);

    private static UserListItemResponse ValidUserListItemResponse() =>
        new("1", "user1@test.com", "user1", "+48123456789", true, new());

    private static string ValidUserId() => "user-123";

    private static List<string> ValidUserIds() => new() { "user-1", "user-2" };

    private static GetUserDetailsResponse ValidUserDetailsResponse() =>
        new("user-123", "user1@test.com", "user1", "+1234567890", true, new());

    private static UpdateUserRequest ValidUpdateUserRequest() =>
        new("user-123", "user1", "user1@test.com", "+1234567890");

    private static GetUserForUpdateResponse ValidUserForUpdateResponse() =>
        new("user-123", "user1", "user1@test.com", "+1234567890");

    [Fact]
    public async Task GetUsersAsync_ReturnsSuccess_WhenFilterIsValid()
    {
        var filter = new GetUsersRequest("test", true, 1, PageSize.Ten);

        var validationResult = new ValidationResult();
        _getUsersRequestValidator.ValidateAsync(filter).Returns(validationResult);

        var userListItem = ValidUserListItemResponse();
        var pagedResult = new Paged<UserListItemResponse>
        {
            Items = new List<UserListItemResponse> { userListItem },
            CurrentPage = 1,
            TotalPages = 1,
            PageSize = PageSize.Ten,
            TotalCount = 1
        };

        _userRepository.GetUsersAsync(Arg.Is(filter.Search), Arg.Is(filter.EmailConfirmed), Arg.Is(filter.PageNumber), filter.PageSize)
            .Returns(pagedResult);

        var result = await _userService.GetUsersAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal("user1", result.Value.Items[0].UserName);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsValidationFailure_WhenFilterIsInvalid()
    {
        var filter = ValidGetUsersRequest() with { PageNumber = 0 };

        var errors = new List<ValidationFailure>
        {
            new ValidationFailure("PageNumber", "Page number must be greater than zero")
        };
        var validationResult = new ValidationResult(errors);

        _getUsersRequestValidator.ValidateAsync(filter).Returns(validationResult);

        var result = await _userService.GetUsersAsync(filter);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("One or more validation errors occurred.", result.Error.Description);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var filter = ValidGetUsersRequest();

        var validationResult = new ValidationResult();
        _getUsersRequestValidator.ValidateAsync(filter).Returns(validationResult);

        _userRepository.GetUsersAsync(filter.Search, filter.EmailConfirmed, filter.PageNumber, filter.PageSize)
            .Returns<Task<Paged<UserListItemResponse>>>(_ => throw new Exception("DB error"));

        var result = await _userService.GetUsersAsync(filter);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.GetUsersFailed, result.Error);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsSuccess_WhenUserExists()
    {
        var userId = ValidUserId();
        var userDetails = ValidUserDetailsResponse();

        _userRepository.GetDetailsAsync(userId).Returns(userDetails);

        var result = await _userService.GetDetailsAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsFailure_WhenUserDoesNotExist()
    {
        var userId = ValidUserId();

        _userRepository.GetDetailsAsync(userId).Throws(new UserNotFoundException(userId));

        var result = await _userService.GetDetailsAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userId = ValidUserId();

        _userRepository.GetDetailsAsync(userId).Returns<Task<GetUserDetailsResponse>>(_ => throw new Exception("DB error"));

        var result = await _userService.GetDetailsAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.GetDetailsAsyncFailed, result.Error);
    }

    [Fact]
    public async Task RemoveSingleUserAsync_ReturnsSuccess_WhenUserIsRemoved()
    {
        var userId = ValidUserId();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.RemoveSingleUserAsync(userId).Returns(Task.CompletedTask);

        var result = await _userService.RemoveSingleUserAsync(userId);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RemoveSingleUserAsync_ReturnsFailure_WhenUserIsAdmin()
    {
        var userId = ValidUserId();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(true);

        var result = await _userService.RemoveSingleUserAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AdminUserDeletionNotAllowed, result.Error);
    }

    [Fact]
    public async Task RemoveSingleUserAsync_ReturnsFailure_WhenUserNotFoundExceptionThrown()
    {
        var userId = ValidUserId();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.RemoveSingleUserAsync(userId)
            .Returns(_ => throw new UserNotFoundException(userId));

        var result = await _userService.RemoveSingleUserAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task RemoveSingleUserAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userId = ValidUserId();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.RemoveSingleUserAsync(userId)
            .Returns(_ => throw new Exception("DB error"));

        var result = await _userService.RemoveSingleUserAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.RemoveSingleUserFailed, result.Error);
    }

    [Fact]
    public async Task RemoveUsersAsync_ReturnsSuccess_WhenAllUsersAreRemoved()
    {
        var userIds = ValidUserIds();

        // All users exist and are not admins
        _userRepository.IsUserInRoleAsync(Arg.Any<string>(), "Admin").Returns(false);
        _userRepository.RemoveUsersAsync(userIds).Returns(Task.CompletedTask);

        var result = await _userService.RemoveUsersAsync(userIds);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RemoveUsersAsync_ReturnsFailure_WhenAnyUserDoesNotExist()
    {
        var userIds = ValidUserIds();

        _userRepository.IsUserInRoleAsync(Arg.Is(userIds[0]), "Admin").Throws(new UserNotFoundException(userIds[0]));

        var result = await _userService.RemoveUsersAsync(userIds);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task RemoveUsersAsync_ReturnsFailure_WhenAnyUserIsAdmin()
    {
        var userIds = ValidUserIds();

        // First user is not admin, second is admin
        _userRepository.IsUserInRoleAsync(userIds[0], "Admin").Returns(false);
        _userRepository.IsUserInRoleAsync(userIds[1], "Admin").Returns(true);

        var result = await _userService.RemoveUsersAsync(userIds);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AdminUserDeletionNotAllowed, result.Error);
    }

    [Fact]
    public async Task RemoveUsersAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userIds = ValidUserIds();

        _userRepository.IsUserInRoleAsync(Arg.Any<string>(), "Admin").Returns(false);
        _userRepository.RemoveUsersAsync(userIds).Returns(_ => throw new Exception("DB error"));

        var result = await _userService.RemoveUsersAsync(userIds);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.RemoveUsersFailed, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsSuccess_WhenUpdateIsValid()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request).Returns(true);

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsValidationFailure_WhenRequestIsInvalid()
    {
        var request = ValidUpdateUserRequest() with { UserName = "" };

        var errors = new List<ValidationFailure>
        {
            new ValidationFailure("UserName", "UserName is required.")
        };
        var validationResult = new ValidationResult(errors);

        _updateUserRequestValidator.Validate(request).Returns(validationResult);

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("One or more validation errors occurred.", result.Error.Description);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenUserIsAdmin()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(true);

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AdminUserUpdateNotAllowed, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenDuplicateUserNameExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request).Throws(new DuplicateUserNameException(request.UserName));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicateUserName, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenDuplicateEmailExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request)
            .Throws(new DuplicateEmailException(request.Email));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicateEmail, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenDuplicatePhoneNumberExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request)
            .Throws(new DuplicatePhoneNumberException(request.PhoneNumber!));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicatePhoneNumber, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenUserNotFoundExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request)
            .Throws(new UserNotFoundException(request.Id));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenUserUpdateExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request)
            .Throws(new UserUpdateException(request.Id, "Failed to update email."));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserUpdateFailed, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFailure_WhenGeneralExceptionThrown()
    {
        var request = ValidUpdateUserRequest();

        var validationResult = new ValidationResult();
        _updateUserRequestValidator.Validate(request).Returns(validationResult);
        _userRepository.IsUserInRoleAsync(request.Id, "Admin").Returns(false);
        _userRepository.UpdateUserAsync(request)
            .Throws(new Exception("Unexpected error"));

        var result = await _userService.UpdateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserUpdateError, result.Error);
    }

    [Fact]
    public async Task GetUserForUpdateAsync_ReturnsSuccess_WhenUserExists()
    {
        var userId = ValidUserId();
        var userForUpdate = ValidUserForUpdateResponse();

        _userRepository.GetUserForUpdateAsync(userId).Returns(userForUpdate);

        var result = await _userService.GetUserForUpdateAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
    }

    [Fact]
    public async Task GetUserForUpdateAsync_ReturnsFailure_WhenUserNotFoundExceptionThrown()
    {
        var userId = ValidUserId();

        _userRepository.GetUserForUpdateAsync(userId)
            .Throws(new UserNotFoundException(userId));

        var result = await _userService.GetUserForUpdateAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetUserForUpdateAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userId = ValidUserId();

        _userRepository.GetUserForUpdateAsync(userId)
            .Throws(new Exception("DB error"));

        var result = await _userService.GetUserForUpdateAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.GetUserForUpdateError, result.Error);
    }
    [Fact]
    public async Task GetUserRolesForUpdateAsync_ReturnsSuccess_WhenUserExists()
    {
        var userId = ValidUserId();
        var assignedRoles = new List<string> { "Admin", "User" };
        var allRoles = new List<IdentityRole>
        {
            new IdentityRole { Id = "1", Name = "Admin" },
            new IdentityRole { Id = "2", Name = "User" },
            new IdentityRole { Id = "3", Name = "Manager" }
        };

        _userRepository.GetUserRolesAsync(userId).Returns(assignedRoles);
        _userRepository.GetAllRolesAsync().Returns(allRoles);

        var result = await _userService.GetUserRolesForUpdateAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var roles = result.Value.ToList();
        Assert.Equal(3, roles.Count);
        Assert.Contains(roles, r => r.Name == "Admin" && r.IsAssigned);
        Assert.Contains(roles, r => r.Name == "User" && r.IsAssigned);
        Assert.Contains(roles, r => r.Name == "Manager" && !r.IsAssigned);
    }

    [Fact]
    public async Task GetUserRolesForUpdateAsync_ReturnsFailure_WhenUserNotFoundExceptionThrown()
    {
        var userId = ValidUserId();

        _userRepository.GetUserRolesAsync(userId)
            .Throws(new UserNotFoundException(userId));

        var result = await _userService.GetUserRolesForUpdateAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetUserRolesForUpdateAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userId = ValidUserId();

        _userRepository.GetUserRolesAsync(userId)
            .Throws(new Exception("DB error"));

        var result = await _userService.GetUserRolesForUpdateAsync(userId);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.GetUserRolesForUpdateFailed, result.Error);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ReturnsSuccess_WhenRolesAreUpdated()
    {
        var userId = ValidUserId();
        var roles = ValidRoles();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.UpdateUserRolesAsync(userId, roles).Returns(Task.CompletedTask);

        var result = await _userService.UpdateUserRolesAsync(userId, roles);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ReturnsFailure_WhenUserIsAdmin()
    {
        var userId = ValidUserId();
        var roles = ValidRoles();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(true);

        var result = await _userService.UpdateUserRolesAsync(userId, roles);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AdminUserUpdateNotAllowed, result.Error);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ReturnsFailure_WhenUserNotFoundExceptionThrown()
    {
        var userId = ValidUserId();
        var roles = ValidRoles();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.UpdateUserRolesAsync(userId, roles)
            .Throws(new UserNotFoundException(userId));

        var result = await _userService.UpdateUserRolesAsync(userId, roles);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ReturnsFailure_WhenRepositoryThrowsException()
    {
        var userId = ValidUserId();
        var roles = ValidRoles();

        _userRepository.IsUserInRoleAsync(userId, "Admin").Returns(false);
        _userRepository.UpdateUserRolesAsync(userId, roles)
            .Throws(new Exception("DB error"));

        var result = await _userService.UpdateUserRolesAsync(userId, roles);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UpdateUserRolesError, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsSuccess_WhenRequestIsValid()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request).Returns(Task.CompletedTask);

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsValidationFailure_WhenRequestIsInvalid()
    {
        var request = ValidCreateUserRequest() with { UserName = "" };

        var errors = new List<ValidationFailure>
        {
            new ValidationFailure("UserName", "User name is required")
        };
        var validationResult = new ValidationResult(errors);

        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("One or more validation errors occurred.", result.Error.Description);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFailure_WhenDuplicateUserNameExceptionThrown()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request)
            .Throws(new DuplicateUserNameException(request.UserName));

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicateUserName, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFailure_WhenDuplicateEmailExceptionThrown()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request)
            .Throws(new DuplicateEmailException(request.Email));

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicateEmail, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFailure_WhenDuplicatePhoneNumberExceptionThrown()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request)
            .Throws(new DuplicatePhoneNumberException(request.PhoneNumber!));

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicatePhoneNumber, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFailure_WhenUserCreationExceptionThrown()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request)
            .Throws(new UserCreationException("Creation failed"));

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.CreateUserFailed, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFailure_WhenGeneralExceptionThrown()
    {
        var request = ValidCreateUserRequest();

        var validationResult = new ValidationResult();
        _createUserRequestValidator.ValidateAsync(request).Returns(validationResult);
        _userRepository.CreateUserAsync(request)
            .Throws(new Exception("Unexpected error"));

        var result = await _userService.CreateUserAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.CreateUserError, result.Error);
    }
}
