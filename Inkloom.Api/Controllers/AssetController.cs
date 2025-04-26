using System.Text.Json;
using Inkloom.Api.Assets;

namespace Inkloom.Api.Controllers;

[ApiController]
[Route(DEFAULT_ROUTE)]
public class AssetController(IAssetManager assetManager) : ControllerBase
{
    private readonly IAssetManager _assetManager = assetManager;

    [HttpGet("{assetId}")]
    public IActionResult Index(string assetId, bool stream = true)
    {
        var asset = _assetManager.GetAsset(assetId.Split(".")[0], stream);

        if (stream ? asset.Stream == null : asset.Bytes.Length == 0)
        {
            return NotFound();
        }

        var currentUser = HttpContext.User.Identity?.Name;

        var metadataJson = string.IsNullOrEmpty(asset.Record.Metadata) ? "{}" : asset.Record.Metadata;
        var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(metadataJson);

        var isAssetPublic = metadata != null && metadata.ContainsKey("public") &&
                            bool.TryParse(metadata["public"].ToString(), out bool isPublic) && isPublic;

        if (currentUser != null && currentUser != asset.Record.Author && !isAssetPublic)
        {
            return Forbid();
        }

        return asset.Stream != null ?
            File(asset.Stream, asset.Record.Mimetype, asset.Record.Name) :
            File(asset.Bytes, asset.Record.Mimetype, asset.Record.Name);
    }
}