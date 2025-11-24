using FluentValidation;
using PhoneNumbers;
using ProductManagementSystem.Application.Requests.Users;

namespace ProductManagementSystem.Application.Validators.Users;
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(256).WithMessage("UserName cannot exceed 256 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .Must(number =>
            {
                if (string.IsNullOrEmpty(number)) return true;

                try
                {
                    var phoneNumber = phoneUtil.Parse(number, "ZZ");
                    return phoneUtil.IsValidNumber(phoneNumber);
                }
                catch (NumberParseException)
                {
                    return false;
                }
            })
            .WithMessage("Enter a valid phone number in international format (e.g., +48...).");
    }
}
