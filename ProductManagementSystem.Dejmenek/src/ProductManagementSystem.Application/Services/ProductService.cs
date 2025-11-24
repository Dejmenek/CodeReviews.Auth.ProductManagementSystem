using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Helpers;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<LaptopRequest> _laptopRequestValidator;
    private readonly IValidator<DesktopRequest> _desktopRequestValidator;
    private readonly IValidator<GetProductsRequest> _getProductsRequestValidator;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<IProductService> logger,
        IValidator<LaptopRequest> laptopRequestValidator,
        IValidator<DesktopRequest> desktopRequestValidator,
        IValidator<GetProductsRequest> getProductsRequestValidator,
        IMemoryCache cache)
    {
        _productRepository = productRepository;
        _logger = logger;
        _laptopRequestValidator = laptopRequestValidator;
        _desktopRequestValidator = desktopRequestValidator;
        _getProductsRequestValidator = getProductsRequestValidator;
        _cache = cache;
    }

    public async Task<Result<Paged<ProductResponse>>> GetProducts(GetProductsRequest request)
    {
        _logger.LogInformation("Getting products: search={Search} page={Page}, pageSize={PageSize}, sortColumn={SortColumn}, sortOrder={SortOrder}",
                    request.Search, request.Page, request.ProductsPerPage, request.SortingProductColumn, request.SortingOrder);

        var validationResult = await _getProductsRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetProductsRequest: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());

            return Result.Failure<Paged<ProductResponse>>(error);
        }

        try
        {
            var products = await _productRepository.GetProducts(
            request.Page, request.Search, request.ProductsPerPage, request.SortingProductColumn, request.SortingOrder);

            var productResponses = products.ToPagedProductResponses();

            _logger.LogInformation("Retrieved {Count} products for page {Page}", productResponses.TotalCount, request.Page);

            return productResponses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products: {@GetProductsRequest}", request);
            return Result.Failure<Paged<ProductResponse>>(ProductErrors.GetProductsFailed);
        }
    }

    public async Task<Result<int>> GetProductsCount()
    {
        _logger.LogInformation("Getting total products count");
        var count = await _productRepository.GetProductsCount();
        _logger.LogInformation("Total products count: {Count}", count);
        return Result.Success(count);
    }

    public async Task<Result> RemoveProducts(List<int> productIds)
    {
        _logger.LogInformation("Removing products: {ProductIds}", productIds);

        try
        {
            await _productRepository.RemoveProducts(productIds);
            _logger.LogInformation("Removed products successfully: {ProductIds}", productIds);

            foreach (var productId in productIds)
            {
                _cache.Remove($"Product_{productId}");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove products: {ProductIds}", productIds);
            return Result.Failure(ProductErrors.RemoveProductsFailed);
        }
    }

    public async Task<Result> RemoveSingleProduct(int productId)
    {
        _logger.LogInformation("Removing single product: {ProductId}", productId);

        try
        {
            await _productRepository.RemoveSingleProduct(productId);
            _logger.LogInformation("Removed product successfully: {ProductId}", productId);

            _cache.Remove($"Product_{productId}");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove product: {ProductId}", productId);
            return Result.Failure(ProductErrors.RemoveSingleProductFailed);
        }
    }

    public async Task<Result> CreateLaptopAsync(LaptopRequest request)
    {
        _logger.LogInformation("Attempting to create laptop: {@LaptopRequest}", request);

        var validationResult = await _laptopRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Laptop request validation failed: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());
            return Result.Failure(error);
        }

        var laptop = request.ToLaptop();

        try
        {
            await _productRepository.AddLaptop(laptop);
            _logger.LogInformation("Laptop created successfully: {@Laptop}", laptop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create laptop: {@LaptopRequest}", request);
            return Result.Failure(ProductErrors.CreateLaptopFailed);
        }

        return Result.Success();
    }

    public async Task<Result> CreateDesktopAsync(DesktopRequest request)
    {
        _logger.LogInformation("Attempting to create desktop: {@DesktopRequest}", request);

        var validationResult = await _desktopRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Desktop request validation failed: {@Errors}", validationResult.Errors);
            var error = new ValidationError(validationResult.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage, ErrorType.Validation)).ToArray());
            return Result.Failure(error);
        }

        var desktop = request.ToDesktop();

        try
        {
            await _productRepository.AddDesktop(desktop);
            _logger.LogInformation("Desktop created successfully: {@Desktop}", desktop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create desktop: {@DesktopRequest}", request);
            return Result.Failure(ProductErrors.CreateDesktopFailed);
        }

        return Result.Success();
    }

    public async Task<Result<Product>> GetProductById(int productId)
    {
        _logger.LogInformation("Getting product by id: {ProductId}", productId);

        string cacheKey = $"Product_{productId}";
        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            _logger.LogInformation("Product retrieved from cache: {ProductId}", productId);
            return Result.Success(cachedProduct!);
        }

        var product = await _productRepository.GetProductById(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", productId);
            return Result.Failure<Product>(ProductErrors.ProductNotFound);
        }

        _logger.LogInformation("Product found: {@Product}", product);
        _cache.Set(cacheKey, product, TimeSpan.FromMinutes(10));
        return Result.Success(product);
    }

    public async Task<Result> UpdateProduct(ProductRequest productRequest, int productId)
    {
        _logger.LogInformation("Updating product: {ProductId} with request: {@ProductRequest}", productId, productRequest);
        try
        {
            Product product = productRequest switch
            {
                LaptopRequest laptopRequest => laptopRequest.ToLaptop(),
                DesktopRequest desktopRequest => desktopRequest.ToDesktop(),
                _ => throw new ArgumentException("Invalid product type")
            };

            await _productRepository.UpdateProduct(product);
            _logger.LogInformation("Product updated successfully: {@Product}", product);

            _cache.Remove($"Product_{productId}");
            return Result.Success();
        }
        catch (ArgumentException)
        {
            _logger.LogWarning("Invalid product type for update: {ProductId} with request: {@ProductRequest}", productId, productRequest);
            return Result.Failure(ProductErrors.InvalidProductType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update product: {ProductId} with request: {@ProductRequest}", productId, productRequest);
            return Result.Failure(ProductErrors.UpdateProductFailed);
        }
    }
}
