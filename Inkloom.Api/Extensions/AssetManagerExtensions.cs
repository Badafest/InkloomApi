using Inkloom.Api.Assets;

namespace Inkloom.Api.Extensions;

public static class FileAssetManagerExtensions
{
    public static IServiceCollection AddFileAssetManager(this IServiceCollection services, FileAssetManagerOptions options)
    {
        services.AddSingleton<IAssetManager>(sp =>
        {
            var recordManager = sp.GetRequiredService<IAssetRecordManager>();
            return new FileAssetManager(recordManager, options);
        });

        return services;
    }
}

public static class SqlDbRecordManagerExtensions
{
    public static IServiceCollection AddSqlDbRecordManager(this IServiceCollection services, SqlDbRecordManagerOptions options)
    {
        var dbRecordManager = new SqlDbRecordManager(options);
        dbRecordManager.EnsureTableExists();
        services.AddSingleton<IAssetRecordManager>(dbRecordManager);
        return services;
    }
}