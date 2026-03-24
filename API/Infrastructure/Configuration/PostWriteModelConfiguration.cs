using API.Core.Domain.WriteModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Configuration;

public class PostWriteModelConfiguration : IEntityTypeConfiguration<PostWriteModel>
{
    public void Configure(EntityTypeBuilder<PostWriteModel> builder)
    {
        builder.ToTable("posts");

        // ValueGeneratedNever: the ID is always a pre-set MongoDB ObjectId string —
        // EF should never attempt to generate or override it.
        builder.Property(p => p.Id).HasMaxLength(24).ValueGeneratedNever();
        builder.Property(p => p.BlogId).HasMaxLength(24);
        builder.Property(p => p.AuthorId).HasMaxLength(256);
        builder.Property(p => p.Title).HasMaxLength(500);
        builder.Property(p => p.Tags).HasDefaultValue("[]");

        builder.HasIndex(p => p.BlogId);
    }
}
