using ProductManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models;

public abstract class CreateProductViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}

public abstract class CreateComputerViewModel : CreateProductViewModel
{
    [Required(ErrorMessage = "Processor brand is required.")]
    [EnumDataType(typeof(Brand), ErrorMessage = "Invalid processor brand.")]
    public Brand Brand { get; set; }

    [Required(ErrorMessage = "Processor model is required.")]
    public string Model { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Processor must have at least 1 core.")]
    public int CoreCount { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Clock speed must be greater than 0.")]
    public double ClockSpeedGHz { get; set; }

    [Required(ErrorMessage = "Ram size is required.")]
    [EnumDataType(typeof(RamSize), ErrorMessage = "Invalid ram size.")]
    public RamSize RamSize { get; set; }

    [Required(ErrorMessage = "Storage capacity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Storage capacity must be greater than zero.")]
    public int StorageCapacity { get; set; }

    [Required(ErrorMessage = "Storage unit is required.")]
    [EnumDataType(typeof(StorageUnit), ErrorMessage = "Invalid storage unit.")]
    public StorageUnit StorageUnit { get; set; }

    [Required(ErrorMessage = "Operating System is required.")]
    [EnumDataType(typeof(SystemType), ErrorMessage = "Invalid operating system.")]
    public SystemType OperatingSystem { get; set; }

    [Required(ErrorMessage = "Graphics card is required.")]
    public string GraphicsCard { get; set; } = string.Empty;
}

public class CreateLaptopViewModel : CreateComputerViewModel
{
    [Required]
    [Range(7.0, 20.0, ErrorMessage = "Screen size must be between 7 and 20 inches.")]
    public double ScreenSize { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Battery life must be at least 1 hour.")]
    public int BatteryLife { get; set; }

    [Required]
    public string WebcamQuality { get; set; } = null!;
}

public class CreateDesktopViewModel : CreateComputerViewModel
{
    [Required]
    [EnumDataType(typeof(CaseType), ErrorMessage = "Invalid case type.")]
    public CaseType CaseType { get; set; }
}
