using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models.Users;

public class RoleCheckboxItem
{
    [Required(ErrorMessage = "Role Id is required.")]
    public string RoleId { get; set; } = null!;
    [Required(ErrorMessage = "Role name is required.")]
    public string RoleName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
