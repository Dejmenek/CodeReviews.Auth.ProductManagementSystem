using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Web.Models;

namespace ProductManagementSystem.Web.Pages.Products.Create;

[Authorize]
public class CreateDesktopModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<CreateDesktopModel> _logger;

    public CreateDesktopModel(IProductService productService, ILogger<CreateDesktopModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty]
    public CreateDesktopViewModel DesktopViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received desktop creation request: {@DesktopViewModel}", DesktopViewModel);

        if (!ModelState.IsValid) return Page();

        var createDesktop = new DesktopRequest
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

        var result = await _productService.CreateDesktopAsync(createDesktop);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Desktop created successfully: {@DesktopRequest}", createDesktop);
            return RedirectToPage("/Products/Index");
        }

        _logger.LogWarning("Desktop creation failed: {@DesktopRequest} | Error: {Error}", createDesktop, result.Error);
        ModelState.AddModelError(string.Empty, "An error occurred while creating the desktop.");
        return Page();
    }
}
