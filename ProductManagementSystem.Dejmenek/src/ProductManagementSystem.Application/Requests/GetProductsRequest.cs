using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Requests;
public record GetProductsRequest(
    string? Search,
    int Page,
    PageSize ProductsPerPage,
    SortingProductColumn SortingProductColumn = SortingProductColumn.Name,
    SortingDirection SortingOrder = SortingDirection.Ascending
);
