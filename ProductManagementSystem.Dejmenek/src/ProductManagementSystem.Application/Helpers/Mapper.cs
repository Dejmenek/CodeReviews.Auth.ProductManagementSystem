using ProductManagementSystem.Application.Requests;
using ProductManagementSystem.Application.Responses;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Application.Helpers;
public static class Mapper
{
    public static Laptop ToLaptop(this LaptopRequest request)
    {
        return new Laptop
        {
            Name = request.Name,
            Price = request.Price,
            IsActive = request.IsActive,
            Processor = request.Processor,
            RamSize = request.RamSize,
            StorageCapacity = request.StorageCapacity,
            OperatingSystem = request.OperatingSystem,
            GraphicsCard = request.GraphicsCard,
            ScreenSize = request.ScreenSize,
            BatteryLife = request.BatteryLife,
            WebcamQuality = request.WebcamQuality
        };
    }

    public static Desktop ToDesktop(this DesktopRequest request)
    {
        return new Desktop
        {
            Name = request.Name,
            Price = request.Price,
            IsActive = request.IsActive,
            Processor = request.Processor,
            RamSize = request.RamSize,
            StorageCapacity = request.StorageCapacity,
            OperatingSystem = request.OperatingSystem,
            GraphicsCard = request.GraphicsCard,
            CaseType = request.CaseType
        };
    }

    public static Paged<ProductResponse> ToPagedProductResponses(this Paged<Product> products)
    {
        return new Paged<ProductResponse>
        {
            Items = products.Items.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name!,
                Price = p.Price,
                DateAdded = p.DateAdded,
                IsActive = p.IsActive,
                EditPageName = p switch
                {
                    Laptop => "UpdateLaptop",
                    Desktop => "UpdateDesktop",
                    _ => "UpdateProduct"
                }
            }).ToList(),
            TotalCount = products.TotalCount,
            CurrentPage = products.CurrentPage,
            TotalPages = products.TotalPages,
            PageSize = products.PageSize
        };
    }
}
