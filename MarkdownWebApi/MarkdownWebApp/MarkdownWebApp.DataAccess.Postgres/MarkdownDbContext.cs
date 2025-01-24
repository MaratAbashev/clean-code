using MarkdownWebApp.DataAccess.Postgres.Configurations;
using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkdownWebApp.DataAccess.Postgres;

public class MarkdownDbContext(DbContextOptions<MarkdownDbContext> options) : DbContext(options)
{
    public DbSet<DocumentEntity> Documents { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<DocumentAccess> DocumentAccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DocumentAccessConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}