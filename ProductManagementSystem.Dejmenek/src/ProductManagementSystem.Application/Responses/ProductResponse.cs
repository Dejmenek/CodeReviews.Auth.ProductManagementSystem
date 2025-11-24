namespace ProductManagementSystem.Application.Responses;
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsActive { get; set; }
    public string EditPageName { get; set; } = string.Empty;
}
