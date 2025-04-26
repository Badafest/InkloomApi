using Inkloom.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Inkloom.Api.Test;

public class DatabaseFixture : IDisposable
{
    public DataContext Context { get; set; } = null!;
    public DatabaseFixture()
    {
        Context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseNpgsql(Startup.config["PgConnectionString"]).Options);

        AddSeedData(SeedData.Users);
    }
    public void AddSeedData<T>(IEnumerable<T> data) where T : class
    {
        Context.Database.EnsureCreated();
        Context.AddRange(data);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("Database Collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }