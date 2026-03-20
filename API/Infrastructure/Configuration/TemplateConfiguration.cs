using API.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Configuration;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        // Only configure what EF can't infer automatically

        // Manual ID assignment (override EF's auto-increment default)
        builder.Property(x => x.Id).ValueGeneratedNever();

        // String length constraints (EF defaults to unlimited)
        //builder.Property(x => x.Name).HasMaxLength(255);

        //builder.Property(x => x.Description).HasMaxLength(1000);

        // Decimal precision (EF default precision may vary by provider)
        //builder.Property(x => x.Price).HasPrecision(18, 2);

        // Database-level default for CreatedAt
        //builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Database-level default for IsActive
        //builder.Property(x => x.IsActive).HasDefaultValue(true);

        // Business logic indexes
        //builder.HasIndex(x => x.Name).IsUnique();
    }
}
