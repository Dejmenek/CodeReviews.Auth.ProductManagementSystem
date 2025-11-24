using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Shared;
using ProductManagementSystem.Web.Models;

namespace ProductManagementSystem.Web.Pages.Products;

[Authorize]
public class ProductsModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsModel> _logger;

    public ProductsModel(IProductService productService, ILogger<ProductsModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public ProductsFilterViewModel Filter { get; set; } = new ProductsFilterViewModel();

    public Paged<ProductResponse> PagedProducts { get; set; } = new Paged<ProductResponse>();

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("Fetching products with filter: {@Filter}", Filter);

        var getProductsRequest = new GetProductsRequest
        (
            Filter.Search,
            Filter.PageNumber,
            Filter.PageSize,
            Filter.SortColumn,
            Filter.SortDirection
        );

        var productsResult = await _productService.GetProducts(getProductsRequest);

        if (productsResult.IsFailure)
        {
            _logger.LogError("Error fetching products: {Error}", productsResult.Error);

            ModelState.AddModelError(string.Empty, "An error occurred while fetching products.");
            return Page();
        }

        PagedProducts = productsResult.Value;
        _logger.LogInformation("Fetched {Count} products for page {CurrentPage}.", PagedProducts.Items.Count, PagedProducts.CurrentPage);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteSingleAsync(int id)
    {
        _logger.LogInformation("Attempting to delete product with id: {ProductId}", id);

        var deleteResult = await _productService.RemoveSingleProduct(id);

        if (deleteResult.IsFailure)
        {
            _logger.LogError("Failed to delete product with id: {ProductId}. Error: {Error}", id, deleteResult.Error);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
        }
        else
        {
            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
        }

        return RedirectToPage(
                null,
                new
                {
                    PageNumber = 1,
                    Filter.PageSize,
                    Filter.SortColumn,
                    Filter.SortDirection,
                    Filter.Search
                }
            );
    }

    public async Task<IActionResult> OnPostDeleteSelectedAsync(List<int> selectedIds)
    {
        _logger.LogInformation("Attempting to delete selected products: {ProductIds}", selectedIds);

        var deleteResult = await _productService.RemoveProducts(selectedIds);

        if (deleteResult.IsFailure)
        {
            _logger.LogError("Failed to delete selected products: {ProductIds}. Error: {Error}", selectedIds, deleteResult.Error);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
        }
        else
        {
            _logger.LogInformation("Selected products deleted successfully: {ProductIds}", selectedIds);
        }

        return RedirectToPage(
            null,
            new
            {
                PageNumber = 1,
                Filter.PageSize,
                Filter.SortColumn,
                Filter.SortDirection,
                Filter.Search
            }
        );
    }
}
