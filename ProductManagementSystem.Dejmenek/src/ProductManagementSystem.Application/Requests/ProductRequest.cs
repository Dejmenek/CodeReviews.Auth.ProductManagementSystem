namespace ProductManagementSystem.Application.Requests;
public abstract class ProductRequest
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
