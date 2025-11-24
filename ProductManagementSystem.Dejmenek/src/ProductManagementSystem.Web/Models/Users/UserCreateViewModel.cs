using ProductManagementSystem.Web.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models.Users;

public class UserCreateViewModel
{
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

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Password must be between {2} and {1} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Please confirm the password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}
