using Inkloom.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Inkloom.Api.Test;

public class DatabaseFixture : IAsyncLifetime
{
    public DataContext Context { get; set; } = null!;
    public DatabaseFixture()
    {
        var connectionString = Startup.config["PgConnectionString"]!;
        Context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options);

    }
    public async ValueTask InitializeAsync()
    {
        await Context.Database.EnsureCreatedAsync();
        Startup.assetRecordManager.EnsureTableExists();
        await AddSeedData(SeedData.Users);
    }


    public async Task AddSeedData<T>(IEnumerable<T> data) where T : class
    {
        await Context.AddRangeAsync(data);
        await Context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
    }
}

[CollectionDefinition("Database Collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }