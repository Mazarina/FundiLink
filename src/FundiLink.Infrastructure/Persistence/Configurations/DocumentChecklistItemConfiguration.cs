using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class DocumentChecklistItemConfiguration : IEntityTypeConfiguration<DocumentChecklistItem>
{
    public void Configure(EntityTypeBuilder<DocumentChecklistItem> builder)
    {
        builder.ToTable("DocumentChecklistItems");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DocumentType).HasConversion<string>();
    }
}
