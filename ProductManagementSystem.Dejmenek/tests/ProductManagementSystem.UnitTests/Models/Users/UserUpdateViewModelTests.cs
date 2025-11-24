using ProductManagementSystem.Web.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.UnitTests.Models.Users;
public class UserUpdateViewModelTests
{
    private IList<ValidationResult> ValidateModel(UserUpdateViewModel model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenIdIsNullOrEmpty()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Invalid user.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenIdIsValid()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Invalid user.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenUserNameIsNullOrEmpty(string userName)
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = userName,
            Email = "user@example.com",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "UserName is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenUserNameTooLong()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = new string('a', 257),
            Email = "user@example.com",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "UserName cannot exceed 256 characters.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenUserNameIsValid()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "UserName is required.");
        Assert.DoesNotContain(results, r => r.ErrorMessage == "UserName cannot exceed 256 characters.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenEmailIsNullOrEmpty(string email)
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = email,
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "invalid-email",
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Enter a valid email address.");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test@wp.pl")]
    public void Validator_ShouldNotHaveError_WhenEmailIsValid(string email)
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = email,
            PhoneNumber = "+48123456789"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Email is required.");
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Enter a valid email address.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsNull()
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = null
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Enter a valid phone number in international format (e.g., +48...).");
    }

    [Theory]
    [InlineData("+48123456789")]
    [InlineData("+12025550123")]
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsValid(string phoneNumber)
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = phoneNumber
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Enter a valid phone number in international format (e.g., +48...).");
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("+123")]
    [InlineData("invalid-phone")]
    public void Validator_ShouldHaveError_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Arrange
        var model = new UserUpdateViewModel
        {
            Id = "123",
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = phoneNumber
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Enter a valid phone number in international format (e.g., +48...).");
    }
}
