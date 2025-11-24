namespace ProductManagementSystem.Infrastructure.Database;

using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Domain;

public static class DatabaseSeeder
{
    public static async Task Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!context.Products.Any())
        {
            var laptops = new[]
            {
                new Laptop
                {
                    Name = "UltraBook Pro 14",
                    Price = 1299.99m,
                    Processor = new Processor(Brand.Intel, "i7-13700H", 12, 3.8),
                    RamSize = RamSize.GB16,
                    StorageCapacity = new StorageCapacity(512, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "NVIDIA RTX 3050",
                    ScreenSize = 14.0,
                    BatteryLife = 10,
                    WebcamQuality = "1080p",
                    IsActive = true
                },
                new Laptop
                {
                    Name = "SlimBook Air 13",
                    Price = 999.99m,
                    Processor = new Processor(Brand.Intel, "i5-1240P", 8, 3.3),
                    RamSize = RamSize.GB8,
                    StorageCapacity = new StorageCapacity(256, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "Intel Iris Xe",
                    ScreenSize = 13.3,
                    BatteryLife = 12,
                    WebcamQuality = "720p",
                    IsActive = true
                },
                new Laptop
                {
                    Name = "PowerBook 15",
                    Price = 1499.99m,
                    Processor = new Processor(Brand.AMD, "Ryzen 7 7840U", 8, 3.5),
                    RamSize = RamSize.GB16,
                    StorageCapacity = new StorageCapacity(1024, StorageUnit.GB),
                    OperatingSystem = SystemType.Linux,
                    GraphicsCard = "AMD Radeon 780M",
                    ScreenSize = 15.6,
                    BatteryLife = 8,
                    WebcamQuality = "1080p",
                    IsActive = true
                },
                new Laptop
                {
                    Name = "StudentBook 14",
                    Price = 599.99m,
                    Processor = new Processor(Brand.Intel, "i3-1115G4", 2, 3.0),
                    RamSize = RamSize.GB8,
                    StorageCapacity = new StorageCapacity(128, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "Intel UHD",
                    ScreenSize = 14.0,
                    BatteryLife = 6,
                    WebcamQuality = "720p",
                    IsActive = true
                }
            };

            var desktops = new[]
            {
                new Desktop
                {
                    Name = "Gaming Tower",
                    Price = 1799.99m,
                    Processor = new Processor(Brand.Intel, "i9-13900K", 24, 5.8),
                    RamSize = RamSize.GB32,
                    StorageCapacity = new StorageCapacity(2048, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "NVIDIA RTX 4080",
                    CaseType = CaseType.FullTower,
                    IsActive = true
                },
                new Desktop
                {
                    Name = "Office Mini",
                    Price = 499.99m,
                    Processor = new Processor(Brand.Intel, "i5-10400", 6, 2.9),
                    RamSize = RamSize.GB8,
                    StorageCapacity = new StorageCapacity(256, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "Intel UHD",
                    CaseType = CaseType.SmallFormFactor,
                    IsActive = true
                },
                new Desktop
                {
                    Name = "Creator Station",
                    Price = 1299.99m,
                    Processor = new Processor(Brand.AMD, "Ryzen 9 7900X", 12, 4.7),
                    RamSize = RamSize.GB16,
                    StorageCapacity = new StorageCapacity(1024, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "NVIDIA RTX 3060",
                    CaseType = CaseType.MidTower,
                    IsActive = true
                },
                new Desktop
                {
                    Name = "Linux Dev Box",
                    Price = 899.99m,
                    Processor = new Processor(Brand.AMD, "Ryzen 5 5600G", 6, 3.9),
                    RamSize = RamSize.GB8,
                    StorageCapacity = new StorageCapacity(512, StorageUnit.GB),
                    OperatingSystem = SystemType.Linux,
                    GraphicsCard = "AMD Radeon Vega",
                    CaseType = CaseType.MiniTower,
                    IsActive = true
                },
                new Desktop
                {
                    Name = "Home Desktop",
                    Price = 699.99m,
                    Processor = new Processor(Brand.Intel, "i3-10100", 4, 3.6),
                    RamSize = RamSize.GB8,
                    StorageCapacity = new StorageCapacity(256, StorageUnit.GB),
                    OperatingSystem = SystemType.Windows,
                    GraphicsCard = "Intel UHD",
                    CaseType = CaseType.MidTower,
                    IsActive = true
                },
                new Desktop
                {
                    Name = "Mac Workstation",
                    Price = 1999.99m,
                    Processor = new Processor(Brand.Apple, "M2", 8, 3.5),
                    RamSize = RamSize.GB16,
                    StorageCapacity = new StorageCapacity(1024, StorageUnit.GB),
                    OperatingSystem = SystemType.MacOS,
                    GraphicsCard = "Apple GPU",
                    CaseType = CaseType.SmallFormFactor,
                    IsActive = true
                }
            };

            context.Laptops.AddRange(laptops);
            context.Desktops.AddRange(desktops);
            await context.SaveChangesAsync();
        }

        if (!userManager.Users.Any())
        {
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var users = new[]
            {
                new { UserName = "admin@test.com", Email = "admin@test.com", Password = "Admin123!", Role = adminRole },
                new { UserName = "admin2@test.com", Email = "admin2@test.com", Password = "Admin456!", Role = adminRole },
                new { UserName = "admin3@test.com", Email = "admin3@test.com", Password = "Admin789!", Role = adminRole },
                new { UserName = "user1@test.com", Email = "user1@test.com", Password = "User123!", Role = "" },
                new { UserName = "user2@test.com", Email = "user2@test.com", Password = "User123!", Role = "" },
                new { UserName = "user3@test.com", Email = "user3@test.com", Password = "User456!", Role = "" },
                new { UserName = "user4@test.com", Email = "user4@test.com", Password = "User789!", Role = "" },
                new { UserName = "user5@test.com", Email = "user5@test.com", Password = "User000!", Role = "" }
            };

            foreach (var u in users)
            {
                var user = new ApplicationUser { UserName = u.UserName, Email = u.Email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, u.Password);
                if (result.Succeeded && !string.IsNullOrEmpty(u.Role))
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                }
            }
        }
    }
}
