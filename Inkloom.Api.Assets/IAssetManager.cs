namespace Inkloom.Api.Assets;

public interface IAssetManager
{
    public Asset GetAsset(string id, bool stream = true);
    public AssetRecord GetAssetRecord(string id);
    public string AddAsset(string path, Asset asset);
    public void RemoveAsset(string id);
    public AssetRecord UpdateAssetRecord(string id, AssetRecord record);
}

