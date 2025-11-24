using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Abstractions;
public interface IProductRepository
{
    Task<Paged<Product>> GetProducts(
        int page,
        string? search,
        PageSize productsPerPage,
        SortingProductColumn sortingProductColumn,
        SortingDirection sortingOrder
    );
    Task<int> GetProductsCount();
    Task RemoveProducts(List<int> productIds);
    Task RemoveSingleProduct(int productId);
    Task AddLaptop(Laptop laptop);
    Task AddDesktop(Desktop desktop);
    Task<Product?> GetProductById(int productId);
    Task UpdateProduct(Product product);
}
