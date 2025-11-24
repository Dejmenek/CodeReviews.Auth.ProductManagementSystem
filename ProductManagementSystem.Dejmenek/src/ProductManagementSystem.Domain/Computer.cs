namespace ProductManagementSystem.Domain;
public abstract class Computer : Product
{
    public Processor Processor { get; set; } = null!;
    public RamSize RamSize { get; set; }
    public StorageCapacity StorageCapacity { get; set; } = null!;
    public SystemType OperatingSystem { get; set; }
    public string GraphicsCard { get; set; } = string.Empty;
}

public enum SystemType
{
    Windows,
    MacOS,
    Linux
}

public enum RamSize
{
    GB8 = 8,
    GB16 = 16,
    GB32 = 32,
    GB64 = 64
}
