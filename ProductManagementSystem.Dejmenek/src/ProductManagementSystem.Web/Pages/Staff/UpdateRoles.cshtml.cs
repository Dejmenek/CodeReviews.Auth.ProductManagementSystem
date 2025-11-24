using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Web.Models.Users;

namespace ProductManagementSystem.Web.Pages.Staff;

[Authorize(Roles = "Admin")]
public class UpdateRolesModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UpdateRolesModel> _logger;

    public UpdateRolesModel(IUserService userService, ILogger<UpdateRolesModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserRolesUpdateViewModel UserRolesUpdateViewModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        var getUserDetailsResult = await _userService.GetDetailsAsync(userId);
        if (!getUserDetailsResult.IsSuccess)
        {
            _logger.LogWarning("User not found for role update: {UserId}", userId);
            return NotFound();
        }

        var getUserRolesForUpdateResult = await _userService.GetUserRolesForUpdateAsync(userId);
        if (!getUserRolesForUpdateResult.IsSuccess)
        {
            _logger.LogWarning("Failed to get roles for user: {UserId}", userId);
            return NotFound();
        }

        var roles = getUserRolesForUpdateResult.Value
            .Select(role => new RoleCheckboxItem
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = role.IsAssigned
            })
            .ToList();

        UserRolesUpdateViewModel = new UserRolesUpdateViewModel
        {
            UserId = getUserDetailsResult.Value.Id,
            UserName = getUserDetailsResult.Value.UserName,
            Roles = roles
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received update roles request: {@UserRolesUpdateViewModel}", UserRolesUpdateViewModel);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var selectedRoles = UserRolesUpdateViewModel.Roles
            .Where(r => r.IsSelected)
            .Select(r => r.RoleName)
            .ToList();

        var updateUserRolesResult = await _userService.UpdateUserRolesAsync(UserRolesUpdateViewModel.UserId, selectedRoles);
        if (!updateUserRolesResult.IsSuccess)
        {
            _logger.LogWarning("Failed to update roles for user: {UserId}", UserRolesUpdateViewModel.UserId);

            ModelState.AddModelError(string.Empty, updateUserRolesResult.Error.Description ?? "An error occurred while updating the user.");

            return Page();
        }

        return RedirectToPage("Details", new { userId = UserRolesUpdateViewModel.UserId });
    }
}
