using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarkdownWebApp.DataAccess.Postgres.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .HasKey(u => u.Id);
        builder
            .HasMany(u => u.UserDocuments)
            .WithOne(da => da.User)
            .HasForeignKey(da => da.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasIndex(p => p.Email)
            .IsUnique();
    }
}