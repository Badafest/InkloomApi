namespace InkloomApi.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        builder.Entity<Token>().HasIndex(t => new { t.UserId, t.Type });
    }

}