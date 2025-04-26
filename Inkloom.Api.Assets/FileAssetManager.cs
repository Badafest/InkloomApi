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

        string directoryPath = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directoryPath);

        asset.Record.FilePath = fullPath;

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
        return _recordManager.AddRecord(asset.Record);
    }

    private static byte[] ReadAsBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    private static FileStream ReadAsStream(string path)
    {
        return File.OpenRead(path);
    }

    public AssetRecord GetAssetRecord(string path)
    {
        return _recordManager.ReadRecord(path);
    }

    public Asset GetAsset(string id, bool stream = true)
    {
        Asset asset = new()
        {
            Record = GetAssetRecord(id)
        };

        if (!Directory.Exists(asset.Record.FilePath))
        {
            return asset;
        }
        if (stream)
        {
            asset.Stream = ReadAsStream(asset.Record.FilePath);
        }
        else
        {
            asset.Bytes = ReadAsBytes(asset.Record.FilePath);
        }
        return asset;
    }

    public void RemoveAsset(string id)
    {
        var record = _recordManager.ReadRecord(id);
        File.Delete(record.FilePath);
        _recordManager.RemoveRecord(record.Id);
    }

    public AssetRecord UpdateAssetRecord(string id, AssetRecord record)
    {
        return _recordManager.UpdateRecord(id, record);
    }
}