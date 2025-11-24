using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Infrastructure.Models;

namespace ProductManagementSystem.Infrastructure.Database;
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Computer> Computers { get; set; }
    public DbSet<Laptop> Laptops { get; set; }
    public DbSet<Desktop> Desktops { get; set; }
    public DbSet<Log> Logs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
