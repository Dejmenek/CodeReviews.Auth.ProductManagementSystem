using FluentValidation;
using ProductManagementSystem.Application.Requests;

namespace ProductManagementSystem.Application.Validators.Products;
public class GetProductsRequestValidator : AbstractValidator<GetProductsRequest>
{
    public GetProductsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.ProductsPerPage)
            .IsInEnum().WithMessage("Invalid page size.");
    }
}
