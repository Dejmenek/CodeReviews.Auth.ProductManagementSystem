using ProductManagementSystem.Web.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.UnitTests.Models.Users;
public class UserCreateViewModelTests
{
    private IList<ValidationResult> ValidateModel(UserCreateViewModel model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenUserNameIsNullOrEmpty(string userName)
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = userName,
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = new string('a', 257),
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = email,
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "invalid-email",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = email,
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = null,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = phoneNumber,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
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
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = phoneNumber,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Enter a valid phone number in international format (e.g., +48...).");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = password,
            ConfirmPassword = password
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Password is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPasswordTooShort()
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "123",
            ConfirmPassword = "123"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Password must be between 8 and 100 characters long.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPasswordTooLong()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = longPassword,
            ConfirmPassword = longPassword
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Password must be between 8 and 100 characters long.");
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("Str0ngP@ssw0rd")]
    public void Validator_ShouldNotHaveError_WhenPasswordIsValid(string password)
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = password,
            ConfirmPassword = password
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Password is required.");
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Password must be between 8 and 100 characters long.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenConfirmPasswordIsNullOrEmpty(string confirmPassword)
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = confirmPassword
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Please confirm the password.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword!"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Passwords do not match.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenPasswordsMatch()
    {
        // Arrange
        var model = new UserCreateViewModel
        {
            UserName = "ValidUser",
            Email = "user@example.com",
            PhoneNumber = "+48123456789",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Passwords do not match.");
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Please confirm the password.");
    }
}
