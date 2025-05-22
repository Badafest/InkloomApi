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

    public static void DeleteOldFile(this IAssetManager assetManager, string apiBaseUrl, string oldLink, string? newLink = null)
    {
        // check if the old file is served by inkloom and the new one is different from it
        // in that case delete the old file as it is no longer used
        var isInkloomAsset = !string.IsNullOrEmpty(oldLink) && oldLink.StartsWith(apiBaseUrl);
        if (oldLink == newLink || !isInkloomAsset) { return; }
        var assetId = oldLink.Split("/")[^1].Split(".")[0];
        if (string.IsNullOrEmpty(assetId)) { return; }
        assetManager.RemoveAsset(assetId);
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