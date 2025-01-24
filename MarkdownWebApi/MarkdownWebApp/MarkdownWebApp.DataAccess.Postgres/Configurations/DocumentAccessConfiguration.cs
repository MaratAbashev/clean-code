using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkdownWebApp.DataAccess.Postgres.Configurations;

public class DocumentAccessConfiguration : IEntityTypeConfiguration<DocumentAccess>
{
    public void Configure(EntityTypeBuilder<DocumentAccess> builder)
    {
        builder
            .HasKey(da => da.Id);
        builder
            .Property(da => da.Role)
            .HasConversion<string>();
        builder
            .HasOne(da => da.Document)
            .WithMany(d => d.UserDocuments)
            .HasForeignKey(da => da.DocumentId);
        builder
            .HasOne(da => da.User)
            .WithMany(u => u.UserDocuments)
            .HasForeignKey(da => da.UserId);
    }
}