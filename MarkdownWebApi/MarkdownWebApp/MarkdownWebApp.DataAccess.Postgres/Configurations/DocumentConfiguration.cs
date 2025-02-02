using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkdownWebApp.DataAccess.Postgres.Configurations;

public class DocumentConfiguration: IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder
            .HasKey(d => d.Id);
        builder
            .Property(d => d.AccessLevel)
            .HasConversion<string>();
        builder
            .HasMany(d => d.UserDocuments)
            .WithOne(da => da.Document)
            .HasForeignKey(da => da.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}