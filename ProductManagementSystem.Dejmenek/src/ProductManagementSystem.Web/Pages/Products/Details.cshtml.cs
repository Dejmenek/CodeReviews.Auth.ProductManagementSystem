using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Web.Pages.Products;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<DetailsModel> _logger;
    public Product Product { get; set; }

    public DetailsModel(IProductService productService, ILogger<DetailsModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(int productId)
    {
        _logger.LogInformation("Received request for product details: {ProductId}", productId);

        var findProductResult = await _productService.GetProductById(productId);

        if (findProductResult.IsSuccess)
        {
            Product = findProductResult.Value;
            _logger.LogInformation("Product found: {@Product}", Product);
            return Page();
        }

        _logger.LogWarning("Product not found: {ProductId}", productId);
        return NotFound();
    }
}
