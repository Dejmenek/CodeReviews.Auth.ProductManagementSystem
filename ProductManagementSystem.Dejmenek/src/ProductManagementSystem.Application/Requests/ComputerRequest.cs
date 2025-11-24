using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Application.Requests;
public abstract class ComputerRequest : ProductRequest
{
    public Processor Processor { get; set; } = null!;
    public RamSize RamSize { get; set; }
    public StorageCapacity StorageCapacity { get; set; } = null!;
    public SystemType OperatingSystem { get; set; }
    public string GraphicsCard { get; set; } = string.Empty;
}
