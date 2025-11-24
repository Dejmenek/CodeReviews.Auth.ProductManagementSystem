using FluentValidation.TestHelper;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Validators.Users;

namespace ProductManagementSystem.UnitTests.Validators.Users;
public class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_ShouldHaveError_WhenUserNameIsNullOrEmpty(string userName)
    {
        // Arrange
        var model = new CreateUserRequest(
            userName,
            "test@gmail.com",
            null,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenUserNameTooLong()
    {
        // Arrange
        var longUserName = new string('a', 257);
        var model = new CreateUserRequest(longUserName, "test@gmail.com", null, "Password123!");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName cannot exceed 256 characters.");
    }

    [Theory]
    [InlineData("TestUser")]
    [InlineData("user_123")]
    public void Validator_ShouldNotHaveError_WhenValidUserName(string userName)
    {
        // Arrange
        var model = new CreateUserRequest(userName, "test@gmail.com", null, "Password123!");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "invalid-email",
            null,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Enter a valid email address.");
    }

    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("test@wp.pl")]
    public void Validator_ShouldNotHaveError_WhenEmailIsValid(string email)
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            email,
            null,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_ShouldHaveError_WhenEmailIsEmpty(string email)
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            email,
            null,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_ShouldHaveError_WhenPasswordIsEmpty(string password)
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "test@gmail.com",
            null,
            password
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPasswordTooShort()
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "test@gmail.com",
            null,
            "123"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenPasswordTooLong()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var model = new CreateUserRequest("TestUser", "test@gmail.com", null, longPassword);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password cannot exceed 100 characters.");
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("Str0ngP@ssw0rd")]
    public void Validator_ShouldNotHaveError_WhenPasswordIsValid(string password)
    {
        // Arrange
        var model = new CreateUserRequest("TestUser", "test@gmail.com", null, password);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsNull()
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "test@gmail.com",
            null,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("+48123456789")] // Valid Polish number
    [InlineData("+12025550123")] // Valid US number
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsValid(string phoneNumber)
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "test@gmail.com",
            phoneNumber,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("123456789")] // Missing country code
    [InlineData("+123")] // Too short
    [InlineData("invalid-phone")] // Not a number
    public void Validator_ShouldHaveError_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Arrange
        var model = new CreateUserRequest(
            "TestUser",
            "test@gmail.com",
            phoneNumber,
            "Password123!"
        );

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Enter a valid phone number in international format (e.g., +48...).");
    }
}
