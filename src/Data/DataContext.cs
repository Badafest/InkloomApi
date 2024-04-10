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
        builder.Entity<User>().HasQueryFilter(u => u.DeletedDate <= DateTime.MinValue);

        builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
        builder.Entity<Blog>().HasQueryFilter(u => u.DeletedDate <= DateTime.MinValue);

        builder.Entity<BlogTag>().HasKey(bt => new { bt.BlogId, bt.TagId });
        builder.Entity<BlogTag>().HasQueryFilter(bt => bt.Blog != null && bt.Blog.DeletedDate <= DateTime.MinValue);
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

            if (dateKey != null)
            {
                entity.Property(dateKey).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync();
    }

}