namespace ProductManagementSystem.Application.Exceptions;
public class DuplicatePhoneNumberException : Exception
{
    public DuplicatePhoneNumberException(string phoneNumber)
        : base($"The phone number '{phoneNumber}' is already in use by another user.")
    {
    }
}
