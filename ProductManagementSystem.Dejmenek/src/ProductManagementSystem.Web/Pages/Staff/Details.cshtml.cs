using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Web.Pages.Staff;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IUserService userService, ILogger<DetailsModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public GetUserDetailsResponse StaffMember { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        _logger.LogInformation("Received request for user details: {UserId}", userId);

        var result = await _userService.GetDetailsAsync(userId);

        if (result.IsFailure)
        {
            if (result.Error.Type == ErrorType.NotFound)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound();
            }

            _logger.LogError("Error retrieving user details for {UserId}: {Error}", userId, result.Error);
            return RedirectToPage("Index");
        }

        StaffMember = result.Value;
        _logger.LogInformation("User found: {@User}", StaffMember);

        return Page();
    }
}
