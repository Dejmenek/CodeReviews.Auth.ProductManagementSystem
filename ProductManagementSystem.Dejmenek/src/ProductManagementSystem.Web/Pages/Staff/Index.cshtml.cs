using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Shared;
using ProductManagementSystem.Web.Models.Users;

namespace ProductManagementSystem.Web.Pages.Staff;

[Authorize(Roles = "Admin")]
public class StaffModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<StaffModel> _logger;

    public StaffModel(IUserService userService, ILogger<StaffModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public UserListFilterViewModel Filter { get; set; } = new UserListFilterViewModel();

    public Paged<UserListItemResponse> PagedUsers { get; set; } = new Paged<UserListItemResponse>();

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("Fetching users with filter: {@Filter}", Filter);

        var getUsersRequest = new GetUsersRequest
        (
            Filter.Search,
            Filter.EmailConfirmed,
            Filter.PageNumber,
            Filter.PageSize
        );

        var usersResult = await _userService.GetUsersAsync(getUsersRequest);

        if (usersResult.IsFailure)
        {
            _logger.LogError("Error fetching users: {Error}", usersResult.Error);
            ModelState.AddModelError(string.Empty, "An error occurred while fetching users.");
            return Page();
        }

        PagedUsers = usersResult.Value;
        _logger.LogInformation("Fetched {UserCount} users.", PagedUsers.Items.Count);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteSingleAsync(string id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);

        var deleteResult = await _userService.RemoveSingleUserAsync(id);

        if (deleteResult.IsFailure)
        {
            _logger.LogError("Error deleting user {UserId}: {Error}", id, deleteResult.Error);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the user.");
        }
        else
        {
            _logger.LogInformation("User {UserId} deleted successfully.", id);
        }

        return RedirectToPage(null, new { PageNumber = 1, Filter.PageSize, Filter.Search });
    }

    public async Task<IActionResult> OnPostDeleteSelectedAsync(List<string> selectedIds)
    {
        _logger.LogInformation("Deleting selected users: {UserIds}", selectedIds);

        var deleteResult = await _userService.RemoveUsersAsync(selectedIds);

        if (deleteResult.IsFailure)
        {
            _logger.LogError("Error deleting selected users: {Error}", deleteResult.Error);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the user.");
        }
        else
        {
            _logger.LogInformation("Selected users deleted successfully.");
        }

        return RedirectToPage(
            null,
            new
            {
                PageNumber = 1,
                Filter.PageSize,
                Filter.Search
            }
        );
    }
}
