using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Domain;
public static class UserErrors
{
    public static readonly Error DuplicateUserName = Error.Conflict("UserService.DuplicateUserName", "This username is already taken.");
    public static readonly Error DuplicateEmail = Error.Conflict("UserService.DuplicateEmail", "This email address is already in use.");
    public static readonly Error DuplicatePhoneNumber = Error.Conflict("UserService.DuplicatePhoneNumber", "This phone number is already associated with another user.");
    public static readonly Error UserNotFound = Error.NotFound("UserService.UserNotFound", "The user could not be found.");
    public static readonly Error UserUpdateFailed = Error.Failure("UserService.UserUpdateFailed", "Failed to update user. Please try again later.");
    public static readonly Error AdminUserUpdateNotAllowed = Error.Conflict("UserService.AdminUserUpdateNotAllowed", "Updating admin users is not allowed.");
    public static readonly Error GetUsersFailed = Error.Failure("UserService.GetUsersFailed", "Failed to retrieve users. Please try again later.");
    public static readonly Error AdminUserDeletionNotAllowed = Error.Conflict("UserService.AdminUserDeletionNotAllowed", "Deleting admin users is not allowed.");
    public static readonly Error GetDetailsAsyncFailed = Error.Failure("UserService.GetDetailsAsyncFailed", "Failed to retrieve user details. Please try again later.");
    public static readonly Error RemoveSingleUserFailed = Error.Failure("UserService.RemoveSingleUserFailed", "Failed to remove user. Please try again later.");
    public static readonly Error RemoveUsersFailed = Error.Failure("UserService.RemoveUsersFailed", "Failed to remove users. Please try again later.");
    public static readonly Error UserUpdateError = Error.Failure("UserService.UserUpdateError", "An error occurred while updating the user. Please try again later.");
    public static readonly Error GetUserForUpdateError = Error.Failure("UserService.GetUserForUpdateError", "An error occurred while retrieving user for update. Please try again later.");
    public static readonly Error GetUserRolesForUpdateFailed = Error.Failure("UserService.GetUserRolesForUpdateFailed", "Failed to retrieve user roles for update. Please try again later.");
    public static readonly Error UpdateUserRolesError = Error.Failure("UserService.UpdateUserRolesError", "Failed to update user roles. Please try again later.");
    public static readonly Error CreateUserFailed = Error.Failure("UserService.CreateUserFailed", "Failed to create user. Please try again later.");
    public static readonly Error CreateUserError = Error.Failure("UserService.CreateUserError", "An error occurred while creating the user. Please try again later.");
}
