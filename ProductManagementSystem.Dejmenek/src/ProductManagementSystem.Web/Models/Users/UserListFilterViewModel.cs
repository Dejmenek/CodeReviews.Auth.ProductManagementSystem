using ProductManagementSystem.Shared;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models.Users;

public class UserListFilterViewModel
{
    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters.")]
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public bool? EmailConfirmed { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be 1 or greater.")]
    public int PageNumber { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.Five;
}
