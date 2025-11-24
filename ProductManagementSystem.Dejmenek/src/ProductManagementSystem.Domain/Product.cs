namespace ProductManagementSystem.Domain;
public abstract class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
