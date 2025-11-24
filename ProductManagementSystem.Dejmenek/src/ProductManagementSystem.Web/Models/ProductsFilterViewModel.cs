using ProductManagementSystem.Shared;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Web.Models;

public class ProductsFilterViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be 1 or greater.")]
    public int PageNumber { get; set; } = 1;
    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters.")]
    public string? Search { get; set; }
    public PageSize PageSize { get; set; } = PageSize.Ten;
    public SortingProductColumn SortColumn { get; set; } = SortingProductColumn.Name;
    public SortingDirection SortDirection { get; set; } = SortingDirection.Ascending;
}
