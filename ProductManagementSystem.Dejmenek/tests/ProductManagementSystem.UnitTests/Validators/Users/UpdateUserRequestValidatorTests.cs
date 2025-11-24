using FluentValidation.TestHelper;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Validators.Users;

namespace ProductManagementSystem.UnitTests.Validators.Users;
public class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator _validator = new();

    [Fact]
    public void Validator_ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var model = new UpdateUserRequest("", "ValidUser", "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Invalid user.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenIdIsNotEmpty()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenUserNameIsEmpty()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "", "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenUserNameExceedsMaxLength()
    {
        // Arrange
        var longUserName = new string('a', 257);
        var model = new UpdateUserRequest("123", longUserName, "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName cannot exceed 256 characters.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenUserNameIsValid()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenEmailIsEmpty()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "invalid-email", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Enter a valid email address.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenEmailIsValid()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", "+48123456789");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsEmpty()
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", null);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("+48123456789")]
    [InlineData("+1 650-253-0000")]
    public void Validator_ShouldNotHaveError_WhenPhoneNumberIsValid(string phoneNumber)
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("not-a-number")]
    [InlineData("+999999999999999999999")]
    public void Validator_ShouldHaveError_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Arrange
        var model = new UpdateUserRequest("123", "ValidUser", "user@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Enter a valid phone number in international format (e.g., +48...).");
    }
}
