namespace ProductManagementSystem.Application.Exceptions;
public class UserUpdateException : Exception
{
    public UserUpdateException(string userId, string reason)
        : base($"Failed to update user with ID '{userId}': {reason}.")
    {
    }
}
