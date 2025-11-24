using ProductManagementSystem.Domain;
using ProductManagementSystem.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.UnitTests.Models.Products;
public class CreateLaptopViewModelTests
{
    private IList<ValidationResult> ValidateModel(CreateLaptopViewModel model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    private CreateLaptopViewModel GetValidModel()
    {
        return new CreateLaptopViewModel
        {
            Name = "Laptop X",
            Price = 999.99m,
            IsActive = true,
            DateAdded = DateTime.UtcNow,
            Brand = Brand.Intel,
            Model = "i7-12700H",
            CoreCount = 8,
            ClockSpeedGHz = 2.5,
            RamSize = RamSize.GB16,
            StorageCapacity = 512,
            StorageUnit = StorageUnit.GB,
            OperatingSystem = SystemType.Windows,
            GraphicsCard = "NVIDIA RTX 3060",
            ScreenSize = 15.6,
            BatteryLife = 8,
            WebcamQuality = "1080p"
        };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenNameIsNullOrEmpty(string name)
    {
        // Arrange
        var model = GetValidModel();
        model.Name = name;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "The Name field is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenPriceIsLessThanZero(int price)
    {
        // Arrange
        var model = GetValidModel();
        model.Price = price;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Price must be greater than zero.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenPriceIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.Price = 100;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Price must be greater than zero.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenModelIsNullOrEmpty(string modelValue)
    {
        // Arrange
        var model = GetValidModel();
        model.Model = modelValue;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Processor model is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenCoreCountIsLessThanOne(int coreCount)
    {
        // Arrange
        var model = GetValidModel();
        model.CoreCount = coreCount;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Processor must have at least 1 core.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenCoreCountIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.CoreCount = 4;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Processor must have at least 1 core.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenClockSpeedIsInvalid(double clockSpeed)
    {
        // Arrange
        var model = GetValidModel();
        model.ClockSpeedGHz = clockSpeed;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Clock speed must be greater than 0.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenClockSpeedIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.ClockSpeedGHz = 2.5;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Clock speed must be greater than 0.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenRamSizeIsInvalid()
    {
        // Arrange
        var model = GetValidModel();
        model.RamSize = (RamSize)999;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Invalid ram size.");
    }

    [Theory]
    [InlineData(RamSize.GB8)]
    [InlineData(RamSize.GB16)]
    [InlineData(RamSize.GB32)]
    [InlineData(RamSize.GB64)]
    public void Validator_ShouldNotHaveError_WhenRamSizeIsValid(RamSize ramSize)
    {
        // Arrange
        var model = GetValidModel();
        model.RamSize = ramSize;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Invalid ram size.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenStorageCapacityIsInvalid(int storageCapacity)
    {
        // Arrange
        var model = GetValidModel();
        model.StorageCapacity = storageCapacity;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Storage capacity must be greater than zero.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenStorageCapacityIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.StorageCapacity = 256;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Storage capacity must be greater than zero.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenStorageUnitIsInvalid()
    {
        // Arrange
        var model = GetValidModel();
        model.StorageUnit = (StorageUnit)999;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Invalid storage unit.");
    }

    [Theory]
    [InlineData(StorageUnit.GB)]
    [InlineData(StorageUnit.TB)]
    public void Validator_ShouldNotHaveError_WhenStorageUnitIsValid(StorageUnit storageUnit)
    {
        // Arrange
        var model = GetValidModel();
        model.StorageUnit = storageUnit;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Invalid storage unit.");
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenOperatingSystemIsInvalid()
    {
        // Arrange
        var model = GetValidModel();
        model.OperatingSystem = (SystemType)999;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Invalid operating system.");
    }

    [Theory]
    [InlineData(SystemType.Windows)]
    [InlineData(SystemType.MacOS)]
    [InlineData(SystemType.Linux)]
    public void Validator_ShouldNotHaveError_WhenOperatingSystemIsValid(SystemType os)
    {
        // Arrange
        var model = GetValidModel();
        model.OperatingSystem = os;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Invalid operating system.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenGraphicsCardIsNullOrEmpty(string graphicsCard)
    {
        // Arrange
        var model = GetValidModel();
        model.GraphicsCard = graphicsCard;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Graphics card is required.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenGraphicsCardIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.GraphicsCard = "NVIDIA RTX 3060";

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Graphics card is required.");
    }

    [Theory]
    [InlineData(6.9)]
    [InlineData(21)]
    public void Validator_ShouldHaveError_WhenScreenSizeIsOutOfRange(double screenSize)
    {
        // Arrange
        var model = GetValidModel();
        model.ScreenSize = screenSize;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Screen size must be between 7 and 20 inches.");
    }

    [Theory]
    [InlineData(7)]
    [InlineData(15.6)]
    [InlineData(20)]
    public void Validator_ShouldNotHaveError_WhenScreenSizeIsValid(double screenSize)
    {
        // Arrange
        var model = GetValidModel();
        model.ScreenSize = screenSize;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Screen size must be between 7 and 20 inches.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_ShouldHaveError_WhenBatteryLifeIsInvalid(int batteryLife)
    {
        // Arrange
        var model = GetValidModel();
        model.BatteryLife = batteryLife;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Battery life must be at least 1 hour.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Validator_ShouldNotHaveError_WhenBatteryLifeIsValid(int batteryLife)
    {
        // Arrange
        var model = GetValidModel();
        model.BatteryLife = batteryLife;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "Battery life must be at least 1 hour.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validator_ShouldHaveError_WhenWebcamQualityIsNullOrEmpty(string webcamQuality)
    {
        // Arrange
        var model = GetValidModel();
        model.WebcamQuality = webcamQuality;

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "The WebcamQuality field is required.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenWebcamQualityIsValid()
    {
        // Arrange
        var model = GetValidModel();
        model.WebcamQuality = "1080p";

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.DoesNotContain(results, r => r.ErrorMessage == "The WebcamQuality field is required.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenAllFieldsAreValid()
    {
        // Arrange
        var model = GetValidModel();

        // Act
        var results = ValidateModel(model);

        // Assert
        Assert.Empty(results);
    }
}
