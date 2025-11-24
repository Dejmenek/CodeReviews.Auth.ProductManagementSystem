using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Web.Models;

namespace ProductManagementSystem.Web.Pages.Products.Update;

[Authorize]
public class UpdateLaptopModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<UpdateLaptopModel> _logger;

    public UpdateLaptopModel(IProductService productService, ILogger<UpdateLaptopModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty]
    public UpdateLaptopViewModel LaptopViewModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        _logger.LogInformation("Received request to update laptop with productId: {ProductId}", productId);

        var getProductByIdResult = await _productService.GetProductById(productId);

        if (!getProductByIdResult.IsSuccess)
        {
            _logger.LogWarning("Product not found for update: {ProductId}", productId);
            return NotFound();
        }

        var product = getProductByIdResult.Value;
        if (product is not Laptop)
        {
            _logger.LogWarning("Product with id {ProductId} is not a laptop.", productId);
            return BadRequest("The specified product is not a laptop.");
        }

        var laptop = (Laptop)product;
        LaptopViewModel = new UpdateLaptopViewModel
        {
            Id = laptop.Id,
            Name = laptop.Name,
            Price = laptop.Price,
            IsActive = laptop.IsActive,
            Brand = laptop.Processor.Brand,
            Model = laptop.Processor.Model,
            CoreCount = laptop.Processor.CoreCount,
            ClockSpeedGHz = laptop.Processor.ClockSpeedGHz,
            RamSize = laptop.RamSize,
            StorageCapacity = laptop.StorageCapacity.Value,
            StorageUnit = laptop.StorageCapacity.Unit,
            OperatingSystem = laptop.OperatingSystem,
            GraphicsCard = laptop.GraphicsCard,
            ScreenSize = laptop.ScreenSize,
            BatteryLife = laptop.BatteryLife,
            WebcamQuality = laptop.WebcamQuality
        };

        _logger.LogInformation("Loaded laptop for update: {@LaptopViewModel}", LaptopViewModel);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received laptop update request: {@LaptopViewModel}", LaptopViewModel);

        if (!ModelState.IsValid) return Page();

        var updateLaptop = new LaptopRequest
        {
            Name = LaptopViewModel.Name,
            Price = LaptopViewModel.Price,
            IsActive = LaptopViewModel.IsActive,
            Processor = new Processor(LaptopViewModel.Brand, LaptopViewModel.Model, LaptopViewModel.CoreCount, LaptopViewModel.ClockSpeedGHz),
            RamSize = LaptopViewModel.RamSize,
            StorageCapacity = new StorageCapacity(LaptopViewModel.StorageCapacity, LaptopViewModel.StorageUnit),
            OperatingSystem = LaptopViewModel.OperatingSystem,
            GraphicsCard = LaptopViewModel.GraphicsCard,
            ScreenSize = LaptopViewModel.ScreenSize,
            BatteryLife = LaptopViewModel.BatteryLife,
            WebcamQuality = LaptopViewModel.WebcamQuality
        };

        var result = await _productService.UpdateProduct(updateLaptop, LaptopViewModel.Id);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Laptop updated successfully: {@LaptopRequest}", updateLaptop);
            return RedirectToPage("/Products/Index");
        }

        _logger.LogWarning("Laptop update failed: {@LaptopRequest} | Error: {Error}", updateLaptop, result.Error);
        ModelState.AddModelError(string.Empty, "An error occurred while creating the laptop.");

        return Page();
    }
}
