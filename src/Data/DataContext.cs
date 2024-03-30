namespace InkloomApi.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<BlogTag> BlogTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        builder.Entity<BlogTag>().HasKey(bt => new { bt.BlogId, bt.TagId });
    }

    public Task<int> SoftSaveChangesAsync(int changesMadeBy)
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            var state = entity.State;
            switch (state)
            {
                case EntityState.Added:
                    entity.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
                    entity.Property("CreatedBy").CurrentValue = changesMadeBy;
                    break;
                case EntityState.Modified:
                    entity.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
                    entity.Property("UpdatedBy").CurrentValue = changesMadeBy;
                    break;
                case EntityState.Deleted:
                    entity.Property("DeletedDate").CurrentValue = DateTime.UtcNow;
                    entity.Property("DeletedBy").CurrentValue = changesMadeBy;
                    entity.State = EntityState.Modified;
                    break;
                default:
                    break;
            }
        }

        return base.SaveChangesAsync();
    }

}