namespace Inkloom.Api.Assets;

public interface IAssetManager
{
    public Asset GetAsset(string path, bool stream = true);
    public AssetRecord GetAssetRecord(string path);
    public string AddAsset(string path, Asset asset);
    public void RemoveAsset(string path);
    public AssetRecord UpdateAssetRecord(string path, AssetRecord record);
}

