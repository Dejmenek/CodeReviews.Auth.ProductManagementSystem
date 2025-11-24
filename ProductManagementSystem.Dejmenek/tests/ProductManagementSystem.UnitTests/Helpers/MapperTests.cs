using ProductManagementSystem.Application.Helpers;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.UnitTests.Helpers;
public class MapperTests
{
    [Fact]
    public void ToLaptop_MapsAllPropertiesCorrectly()
    {
        var request = new LaptopRequest
        {
            Name = "Test Laptop",
            Price = 999.99m,
            IsActive = true,
            Processor = new Processor(Brand.Intel, "Intel I7", 16, 14.0),
            RamSize = RamSize.GB16,
            StorageCapacity = new StorageCapacity(256, StorageUnit.GB),
            OperatingSystem = SystemType.Windows,
            GraphicsCard = "NVIDIA GTX 1650",
            ScreenSize = 15.6,
            BatteryLife = 10,
            WebcamQuality = "HD"
        };

        var laptop = request.ToLaptop();

        Assert.Equal(request.Name, laptop.Name);
        Assert.Equal(request.Price, laptop.Price);
        Assert.Equal(request.IsActive, laptop.IsActive);
        Assert.Equal(request.Processor, laptop.Processor);
        Assert.Equal(request.RamSize, laptop.RamSize);
        Assert.Equal(request.StorageCapacity, laptop.StorageCapacity);
        Assert.Equal(request.OperatingSystem, laptop.OperatingSystem);
        Assert.Equal(request.GraphicsCard, laptop.GraphicsCard);
        Assert.Equal(request.ScreenSize, laptop.ScreenSize);
        Assert.Equal(request.BatteryLife, laptop.BatteryLife);
        Assert.Equal(request.WebcamQuality, laptop.WebcamQuality);
    }

    [Fact]
    public void ToDesktop_MapsAllPropertiesCorrectly()
    {
        var request = new DesktopRequest
        {
            Name = "Test Desktop",
            Price = 799.99m,
            IsActive = false,
            Processor = new Processor(Brand.AMD, "AMD_Ryzen7", 16, 24.0),
            RamSize = RamSize.GB32,
            StorageCapacity = new StorageCapacity(256, StorageUnit.GB),
            OperatingSystem = SystemType.Linux,
            GraphicsCard = "AMD Radeon RX 5700",
            CaseType = CaseType.MiniTower
        };

        var desktop = request.ToDesktop();

        Assert.Equal(request.Name, desktop.Name);
        Assert.Equal(request.Price, desktop.Price);
        Assert.Equal(request.IsActive, desktop.IsActive);
        Assert.Equal(request.Processor, desktop.Processor);
        Assert.Equal(request.RamSize, desktop.RamSize);
        Assert.Equal(request.StorageCapacity, desktop.StorageCapacity);
        Assert.Equal(request.OperatingSystem, desktop.OperatingSystem);
        Assert.Equal(request.GraphicsCard, desktop.GraphicsCard);
        Assert.Equal(request.CaseType, desktop.CaseType);
    }
}