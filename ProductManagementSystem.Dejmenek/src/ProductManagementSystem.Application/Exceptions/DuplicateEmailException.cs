namespace ProductManagementSystem.Application.Exceptions;
public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"The email '{email}' is already in use by another user.")
    {
    }
}
