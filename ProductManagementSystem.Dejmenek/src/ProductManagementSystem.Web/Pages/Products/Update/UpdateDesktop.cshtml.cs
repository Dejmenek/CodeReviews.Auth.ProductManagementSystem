using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Web.Models;

namespace ProductManagementSystem.Web.Pages.Products.Update;

[Authorize]
public class UpdateDesktopModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<UpdateDesktopModel> _logger;

    public UpdateDesktopModel(IProductService productService, ILogger<UpdateDesktopModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty]
    public UpdateDesktopViewModel DesktopViewModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        _logger.LogInformation("Received request to update desktop with productId: {ProductId}", productId);

        var getProductByIdResult = await _productService.GetProductById(productId);

        if (!getProductByIdResult.IsSuccess)
        {
            _logger.LogWarning("Product not found for update: {ProductId}", productId);
            return NotFound();
        }

        var product = getProductByIdResult.Value;

        if (product is not Desktop)
        {
            _logger.LogWarning("Product with id {ProductId} is not a desktop.", productId);
            return BadRequest("The specified product is not a desktop.");
        }

        var desktop = (Desktop)product;
        DesktopViewModel = new UpdateDesktopViewModel
        {
            Id = desktop.Id,
            Name = desktop.Name,
            Price = desktop.Price,
            IsActive = desktop.IsActive,
            Brand = desktop.Processor.Brand,
            Model = desktop.Processor.Model,
            CoreCount = desktop.Processor.CoreCount,
            ClockSpeedGHz = desktop.Processor.ClockSpeedGHz,
            RamSize = desktop.RamSize,
            StorageCapacity = desktop.StorageCapacity.Value,
            StorageUnit = desktop.StorageCapacity.Unit,
            OperatingSystem = desktop.OperatingSystem,
            GraphicsCard = desktop.GraphicsCard,
            CaseType = desktop.CaseType
        };

        _logger.LogInformation("Loaded desktop for update: {@DesktopViewModel}", DesktopViewModel);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received desktop update request: {@DesktopViewModel}", DesktopViewModel);

        if (!ModelState.IsValid) return Page();

        var updateDesktop = new DesktopRequest
        {
            Name = DesktopViewModel.Name,
            Price = DesktopViewModel.Price,
            IsActive = DesktopViewModel.IsActive,
            Processor = new Processor(DesktopViewModel.Brand, DesktopViewModel.Model, DesktopViewModel.CoreCount, DesktopViewModel.ClockSpeedGHz),
            RamSize = DesktopViewModel.RamSize,
            StorageCapacity = new StorageCapacity(DesktopViewModel.StorageCapacity, DesktopViewModel.StorageUnit),
            OperatingSystem = DesktopViewModel.OperatingSystem,
            GraphicsCard = DesktopViewModel.GraphicsCard,
            CaseType = DesktopViewModel.CaseType
        };

        var result = await _productService.UpdateProduct(updateDesktop, DesktopViewModel.Id);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Desktop updated successfully: {@DesktopRequest}", updateDesktop);
            return RedirectToPage("/Products/Index");
        }

        _logger.LogWarning("Desktop update failed: {@DesktopRequest} | Error: {Error}", updateDesktop, result.Error);
        ModelState.AddModelError(string.Empty, "An error occurred while creating the desktop.");
        return Page();
    }
}
