using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Infrastructure.Configurations;
internal sealed class ComputerConfiguration : IEntityTypeConfiguration<Computer>
{
    public void Configure(EntityTypeBuilder<Computer> builder)
    {
        builder.Property(c => c.GraphicsCard).IsRequired().HasMaxLength(100);

        builder.Property(c => c.RamSize).IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.OperatingSystem).IsRequired()
            .HasMaxLength(25)
            .HasConversion<string>();

        builder.OwnsOne(c => c.StorageCapacity, sa =>
        {
            sa.Property(s => s.Value).HasColumnName("StorageValue").IsRequired();
            sa.Property(s => s.Unit).HasColumnName("StorageUnit")
                                    .IsRequired()
                                    .HasMaxLength(10)
                                    .HasConversion<string>();
        });

        builder.OwnsOne(c => c.Processor, pa =>
        {
            pa.Property(p => p.Brand)
                .HasColumnName("ProcessorBrand")
                .IsRequired()
                .HasMaxLength(25)
                .HasConversion<string>();
            pa.Property(p => p.Model).HasColumnName("ProcessorModel").IsRequired().HasMaxLength(100);
            pa.Property(p => p.CoreCount).HasColumnName("ProcessorCoreCount").IsRequired();
            pa.Property(p => p.ClockSpeedGHz).HasColumnName("ProcessorClockSpeedGHz").IsRequired();
        });
    }
}
