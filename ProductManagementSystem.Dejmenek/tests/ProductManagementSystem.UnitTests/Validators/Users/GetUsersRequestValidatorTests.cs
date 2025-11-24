using FluentValidation.TestHelper;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Validators.Users;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.UnitTests.Validators.Users;
public class GetUsersRequestValidatorTests
{
    private readonly GetUsersRequestValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenPageNumberIsZeroOrNegative(int pageNumber)
    {
        // Arrange
        var model = new GetUsersRequest(null, null, pageNumber, PageSize.Ten);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than 0.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validator_ShouldNotHaveError_WhenPageNumberIsValid(int pageNumber)
    {
        // Arrange
        var model = new GetUsersRequest(null, null, pageNumber, PageSize.Ten);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(PageSize.Two)]
    [InlineData(PageSize.Five)]
    [InlineData(PageSize.Ten)]
    [InlineData(PageSize.Hundred)]
    public void Validator_ShouldNotHaveError_WhenPageSizeIsValid(PageSize pageSize)
    {
        // Arrange
        var model = new GetUsersRequest(null, null, 1, pageSize);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPageSizeIsInvalid()
    {
        // Arrange
        var invalidPageSize = (PageSize)999;
        var model = new GetUsersRequest(null, null, 1, invalidPageSize);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Invalid page size.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenSearchAndEmailConfirmedAreNull()
    {
        // Arrange
        var model = new GetUsersRequest(null, null, 1, PageSize.Ten);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenSearchAndEmailConfirmedAreSet()
    {
        // Arrange
        var model = new GetUsersRequest("search", true, 1, PageSize.Five);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
