namespace Inkloom.Api.Assets;

public interface IAssetRecordManager
{
    public string AddRecord(AssetRecord record);
    public AssetRecord ReadRecord(string id);
    public AssetRecord UpdateRecord(string id, AssetRecord record);
    public void RemoveRecord(string id);
}