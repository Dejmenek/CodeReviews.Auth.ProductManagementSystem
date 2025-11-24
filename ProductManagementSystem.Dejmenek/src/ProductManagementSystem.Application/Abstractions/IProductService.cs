using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Abstractions;
public interface IProductService
{
    Task<Result<Paged<ProductResponse>>> GetProducts(GetProductsRequest request);
    Task<Result<int>> GetProductsCount();
    Task<Result> RemoveProducts(List<int> productIds);
    Task<Result> RemoveSingleProduct(int productId);
    Task<Result> CreateLaptopAsync(LaptopRequest request);
    Task<Result> CreateDesktopAsync(DesktopRequest request);
    Task<Result<Product>> GetProductById(int productId);
    Task<Result> UpdateProduct(ProductRequest productm, int productId);
}
