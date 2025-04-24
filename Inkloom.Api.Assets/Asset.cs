namespace Inkloom.Api.Assets
{
    public enum AssetType
    {
        Image, Audio, Video, Document
    }

    public class AssetRecord
    {
        public string Id { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public AssetType Type { get; set; }
        public string Mimetype { get; set; } = "";
        public long Size { get; set; }
        public string Metadata { get; set; } = "";
    }
    public class Asset
    {
        public AssetRecord Record { get; set; } = new();
        public Stream? Stream { get; set; }
        public byte[] Bytes { get; set; } = [];
    }
}
