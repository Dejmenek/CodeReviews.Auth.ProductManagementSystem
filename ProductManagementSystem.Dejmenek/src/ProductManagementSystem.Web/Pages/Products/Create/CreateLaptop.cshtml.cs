using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Web.Models;

namespace ProductManagementSystem.Web.Pages.Products.Create;

[Authorize]
public class CreateLaptopModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<CreateLaptopModel> _logger;

    public CreateLaptopModel(IProductService productService, ILogger<CreateLaptopModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty]
    public CreateLaptopViewModel LaptopViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received laptop creation request: {@LaptopViewModel}", LaptopViewModel);

        if (!ModelState.IsValid) return Page();

        var createLaptop = new LaptopRequest
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

        var result = await _productService.CreateLaptopAsync(createLaptop);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Laptop created successfully: {@LaptopRequest}", createLaptop);
            return RedirectToPage("/Products/Index");
        }

        _logger.LogWarning("Laptop creation failed: {@LaptopRequest} | Error: {Error}", createLaptop, result.Error);
        ModelState.AddModelError(string.Empty, "An error occurred while creating the laptop.");

        return Page();
    }
}
