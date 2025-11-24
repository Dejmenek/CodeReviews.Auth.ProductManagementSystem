using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Attributes;

public sealed class LibPhoneNumberAttribute : ValidationAttribute
{
    private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();

    public override bool IsValid(object? value)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return true;
        }

        try
        {
            var parsedNumber = phoneUtil.Parse(value.ToString(), "ZZ");
            return phoneUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
}
