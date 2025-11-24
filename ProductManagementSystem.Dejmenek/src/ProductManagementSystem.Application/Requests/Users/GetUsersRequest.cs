using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Requests.Users;
public record GetUsersRequest(
    string? Search,
    bool? EmailConfirmed,
    int PageNumber,
    PageSize PageSize
);
