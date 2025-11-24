namespace ProductManagementSystem.Application.Exceptions;
public class DuplicateUserNameException : Exception
{
    public DuplicateUserNameException(string userName)
        : base($"The username '{userName}' is already taken.")
    {
    }
}
