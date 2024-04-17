namespace Inkloom.Api.Test;

public static class Seeder
{
    private static readonly TestDataContext dataContext = new(new(), Configuration.config);
    public static async Task Seed<T>(IEnumerable<T> data) where T : class
    {
        await dataContext.Set<T>().AddRangeAsync(data);
        await dataContext.SoftSaveChangesAsync();
    }
}
