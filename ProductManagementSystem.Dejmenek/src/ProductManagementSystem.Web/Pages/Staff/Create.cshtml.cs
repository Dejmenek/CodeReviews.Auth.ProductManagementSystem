using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Web.Models.Users;

namespace ProductManagementSystem.Web.Pages.Staff;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IUserService userService, ILogger<CreateModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserCreateViewModel UserCreateViewModel { get; set; } = new UserCreateViewModel();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Creating a new user");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var createUserRequest = new CreateUserRequest
        (
            UserCreateViewModel.UserName,
            UserCreateViewModel.Email,
            UserCreateViewModel.PhoneNumber,
            UserCreateViewModel.Password
        );

        var result = await _userService.CreateUserAsync(createUserRequest);
        if (!result.IsSuccess)
        {
            switch (result.Error.Code)
            {
                case "UserService.DuplicateUserName":
                    ModelState.AddModelError("UserCreateViewModel.UserName", "This username is already taken.");
                    break;
                case "UserService.DuplicateEmail":
                    ModelState.AddModelError("UserCreateViewModel.Email", "This email address is already in use.");
                    break;
                case "UserService.DuplicatePhoneNumber":
                    ModelState.AddModelError("UserCreateViewModel.PhoneNumber", "This phone number is already associated with another user.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, result.Error.Description ?? "An error occurred while creating the user.");
                    break;
            }
            return Page();
        }

        _logger.LogInformation("User created successfully");
        return RedirectToPage("Index");
    }
}
