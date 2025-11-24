using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Services;

namespace ProductManagementSystem.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
