namespace Inkloom.Api.Assets;

public interface IAssetRecordManager
{
    public string AddRecord(AssetRecord asset);
    public AssetRecord ReadRecord(string id);
    public AssetRecord UpdateRecord(string id, AssetRecord asset);
    public void RemoveRecord(string id);
}