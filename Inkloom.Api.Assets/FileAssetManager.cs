namespace Inkloom.Api.Assets;

public class FileAssetManagerOptions
{
    public string BaseDirectory = "./";
    public bool GroupByAssetType = true;
}
public class FileAssetManager(IAssetRecordManager recordManager, FileAssetManagerOptions options) : IAssetManager
{
    private readonly IAssetRecordManager _recordManager = recordManager;
    private readonly FileAssetManagerOptions _options = options;
    public string AddAsset(string path, Asset asset)
    {
        var location = _options.GroupByAssetType ? Path.Combine(_options.BaseDirectory, asset.Record.Type.ToString()) : _options.BaseDirectory;
        var fullPath = Path.Combine(location, path);

        asset.Record.Id = fullPath;

        if (asset.Stream != null)
        {
            using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            asset.Stream.CopyTo(fileStream);
        }
        else if (asset.Bytes != null)
        {
            File.WriteAllBytes(fullPath, asset.Bytes);
        }
        else
        {
            throw new InvalidOperationException("Asset must have either Stream or Bytes data.");
        }

        asset.Record.Size = asset.Stream != null ? asset.Stream.Length : asset.Bytes.Length;
        _recordManager.AddRecord(asset.Record);
        return asset.Record.Id;
    }

    public static byte[] ReadAsBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    public static Stream ReadAsStream(string path)
    {
        return File.OpenRead(path);
    }

    public AssetRecord GetAssetRecord(string path)
    {
        return _recordManager.ReadRecord(path);
    }

    public Asset GetAsset(string path, bool stream = true)
    {
        Asset asset = new()
        {
            Record = GetAssetRecord(path)
        };
        if (stream)
        {
            asset.Stream = ReadAsStream(asset.Record.Id);
        }
        else
        {
            asset.Bytes = ReadAsBytes(asset.Record.Id);
        }
        return asset;
    }

    public void RemoveAsset(string path)
    {
        File.Delete(path);
        _recordManager.RemoveRecord(path);
    }

    public AssetRecord UpdateAssetRecord(string path, AssetRecord record)
    {
        return _recordManager.UpdateRecord(path, record);
    }
}