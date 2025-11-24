using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Infrastructure.Configurations;
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.DateAdded).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasDiscriminator<string>("ProductType")
               .HasValue<Product>("Product")
               .HasValue<Computer>("Computer")
               .HasValue<Laptop>("Laptop")
               .HasValue<Desktop>("Desktop");
    }
}
