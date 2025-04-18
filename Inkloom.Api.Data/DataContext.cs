using Inkloom.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Inkloom.Api.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    // constructor without parameter for migration
    public DataContext() : this(new DbContextOptionsBuilder<DataContext>().UseNpgsql(Environment.GetEnvironmentVariable("PGSQLCONSTR")).Options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Tag> Tags { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        builder.Entity<User>().HasQueryFilter(u => u.DeletedDate <= DateTime.MinValue);

        builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
        builder.Entity<Blog>().HasQueryFilter(u => u.DeletedDate <= DateTime.MinValue);

        builder.Entity<Blog>().HasMany(b => b.Tags).WithMany(t => t.Blogs).UsingEntity<BlogTag>();
        builder.Entity<Tag>().HasMany(t => t.Blogs).WithMany(b => b.Tags).UsingEntity<BlogTag>();
        builder.Entity<BlogTag>().HasQueryFilter(bt => bt.Blog.DeletedDate <= DateTime.MinValue);
        builder.Entity<Token>().HasQueryFilter(t => t.User != null && t.User.DeletedDate <= DateTime.MinValue);
    }

    public Task<int> SoftSaveChangesAsync()
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            var state = entity.State;
            string? dateKey = null;

            switch (state)
            {
                case EntityState.Added:
                    dateKey = "CreatedDate";
                    break;
                case EntityState.Modified:
                    dateKey = "UpdatedDate";
                    break;
                case EntityState.Deleted:
                    entity.State = EntityState.Modified;
                    dateKey = "DeletedDate";
                    break;
                default:
                    break;
            }

            if (dateKey != null && entity.Entity.GetType().GetProperty(dateKey) != null)
            {
                entity.Property(dateKey).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync();
    }

}