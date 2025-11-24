using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models.Users;

public class UserRolesUpdateViewModel
{
    [Required(ErrorMessage = "Invalid user.")]
    public string UserId { get; set; } = null!;
    [Display(Name = "User Name")]
    public string UserName { get; set; } = string.Empty;
    public List<RoleCheckboxItem> Roles { get; set; } = new();
}
