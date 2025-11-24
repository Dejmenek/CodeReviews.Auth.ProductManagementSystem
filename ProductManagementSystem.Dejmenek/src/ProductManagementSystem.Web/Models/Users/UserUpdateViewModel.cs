using ProductManagementSystem.Web.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models.Users;

public class UserUpdateViewModel
{
    [Required(ErrorMessage = "Invalid user.")]
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "UserName is required.")]
    [StringLength(256, ErrorMessage = "UserName cannot exceed 256 characters.")]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [LibPhoneNumber(ErrorMessage = "Enter a valid phone number in international format (e.g., +48...).")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}
