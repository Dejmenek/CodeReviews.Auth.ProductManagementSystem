using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Infrastructure.Configurations;
internal sealed class LaptopConfiguration : IEntityTypeConfiguration<Laptop>
{
    public void Configure(EntityTypeBuilder<Laptop> builder)
    {
        builder.Property(l => l.ScreenSize)
               .IsRequired();
        builder.Property(l => l.BatteryLife)
               .IsRequired();
        builder.Property(l => l.WebcamQuality)
                .IsRequired()
                .HasMaxLength(50);
    }
}
