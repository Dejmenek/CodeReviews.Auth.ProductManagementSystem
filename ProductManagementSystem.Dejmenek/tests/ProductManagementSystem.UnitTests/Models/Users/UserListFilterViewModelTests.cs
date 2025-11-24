using ProductManagementSystem.Shared;
using ProductManagementSystem.Web.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.UnitTests.Models.Users;
public class UserListFilterViewModelTests
{
    private IList<ValidationResult> ValidateModel(UserListFilterViewModel model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenSearchIsNull()
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = 1,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Search text cannot exceed 100 characters.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenSearchIsTooLong()
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = new string('a', 101),
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = 1,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Search text cannot exceed 100 characters.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenSearchIsValidLength()
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = new string('a', 100),
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = 1,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Search text cannot exceed 100 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenPageNumberIsLessThanOne(int pageNumber)
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = pageNumber,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Page number must be 1 or greater.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Validator_ShouldNotHaveError_WhenPageNumberIsValid(int pageNumber)
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = pageNumber,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Page number must be 1 or greater.");
    }

    [Theory]
    [InlineData(PageSize.Two)]
    [InlineData(PageSize.Five)]
    [InlineData(PageSize.Ten)]
    [InlineData(PageSize.Hundred)]
    public void Validator_ShouldNotHaveError_WhenPageSizeIsValid(PageSize pageSize)
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = 1,
            PageSize = pageSize
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenIsActiveAndEmailConfirmedAreNull()
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = null,
            EmailConfirmed = null,
            PageNumber = 1,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenIsActiveAndEmailConfirmedAreSet()
    {
        // Arrange
        var model = new UserListFilterViewModel
        {
            Search = null,
            IsActive = true,
            EmailConfirmed = false,
            PageNumber = 1,
            PageSize = PageSize.Five
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Empty(results);
    }
}
