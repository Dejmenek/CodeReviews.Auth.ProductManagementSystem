using FluentValidation;
using ProductManagementSystem.Application.Requests.Users;

namespace ProductManagementSystem.Application.Validators.Users;
public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .IsInEnum().WithMessage("Invalid page size.");
    }
}
