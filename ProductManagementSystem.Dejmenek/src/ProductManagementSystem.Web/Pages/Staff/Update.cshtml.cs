using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Web.Models.Users;

namespace ProductManagementSystem.Web.Pages.Staff;

[Authorize(Roles = "Admin")]
public class UpdateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UpdateModel> _logger;

    public UpdateModel(IUserService userService, ILogger<UpdateModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserUpdateViewModel UserUpdateViewModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        _logger.LogInformation("Received request to update user with userId: {UserId}", userId);

        var getUserForUpdateResult = await _userService.GetUserForUpdateAsync(userId);
        if (!getUserForUpdateResult.IsSuccess)
        {
            _logger.LogWarning("User not found for update: {UserId}", userId);
            return NotFound();
        }

        var user = getUserForUpdateResult.Value;
        UserUpdateViewModel = new UserUpdateViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Received update user request: {@UserUpdateViewModel}", UserUpdateViewModel);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updateUserRequest = new UpdateUserRequest
        (
            UserUpdateViewModel.Id,
            UserUpdateViewModel.UserName,
            UserUpdateViewModel.Email,
            UserUpdateViewModel.PhoneNumber
        );

        var updateUserResult = await _userService.UpdateUserAsync(updateUserRequest);
        if (!updateUserResult.IsSuccess)
        {
            _logger.LogWarning("Failed to update user: {@Error}", updateUserResult.Error);
            switch (updateUserResult.Error.Code)
            {
                case "UserService.DuplicateUserName":
                    ModelState.AddModelError("UserUpdateViewModel.UserName", "This username is already taken.");
                    break;
                case "UserService.DuplicateEmail":
                    ModelState.AddModelError("UserUpdateViewModel.Email", "This email address is already in use.");
                    break;
                case "UserService.DuplicatePhoneNumber":
                    ModelState.AddModelError("UserUpdateViewModel.PhoneNumber", "This phone number is already associated with another user.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, updateUserResult.Error.Description ?? "An error occurred while updating the user.");
                    break;
            }
            return Page();
        }

        _logger.LogInformation("Successfully updated user with id: {UserId}", UserUpdateViewModel.Id);
        return RedirectToPage("/Staff/Index");
    }
}
