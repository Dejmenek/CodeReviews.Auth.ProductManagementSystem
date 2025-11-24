using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Infrastructure.Configurations;
internal sealed class DesktopConfiguration : IEntityTypeConfiguration<Desktop>
{
    public void Configure(EntityTypeBuilder<Desktop> builder)
    {
        builder.Property(d => d.CaseType).HasConversion<string>().IsRequired().HasMaxLength(25);
    }
}
