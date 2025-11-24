using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Domain;
public static class ProductErrors
{
    public static readonly Error ProductNotFound =
        Error.NotFound("ProductService.ProductNotFound", "The product could not be found.");

    public static readonly Error InvalidPage =
        Error.Failure("ProductService.InvalidPage", "Page number must be greater than zero.");

    public static readonly Error RemoveSingleProductFailed =
        Error.Failure("ProductService.RemoveSingleProductFailed", "An error occurred while deleting the product.");

    public static readonly Error RemoveProductsFailed =
        Error.Failure("ProductService.RemoveProductsFailed", "An error occurred while deleting products.");

    public static readonly Error CreateLaptopFailed =
        Error.Failure("ProductService.CreateLaptopFailed", "An error occurred while creating the laptop.");

    public static readonly Error CreateDesktopFailed =
        Error.Failure("ProductService.CreateDesktopFailed", "An error occurred while creating the desktop.");

    public static readonly Error UpdateProductFailed =
        Error.Failure("ProductService.UpdateProductFailed", "An error occurred while updating the product.");

    public static readonly Error InvalidProductType =
        Error.Problem("ProductService.InvalidProductType", "The specified product type is invalid.");
    public static readonly Error GetProductsFailed =
        Error.Failure("ProductService.GetProductsFailed", "An error occurred while fetching products.");
}
