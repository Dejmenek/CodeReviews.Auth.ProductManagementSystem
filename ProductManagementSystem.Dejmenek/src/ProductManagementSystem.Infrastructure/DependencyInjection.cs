using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Infrastructure.Database;
using ProductManagementSystem.Infrastructure.Repositories;
using ProductManagementSystem.Infrastructure.Services;

namespace ProductManagementSystem.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDatabase(configuration)
            .AddServices()
            .AddRepositories();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(connectionString)
            );
        return services;
    }
}
