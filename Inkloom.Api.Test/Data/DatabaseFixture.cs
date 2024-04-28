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
    public async void AddSeedData<T>(IEnumerable<T> data) where T : class
    {
        await Context.Database.EnsureCreatedAsync();
        await Context.AddRangeAsync(data);
        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("Database Collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }