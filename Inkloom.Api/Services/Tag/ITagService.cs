namespace Inkloom.Api.Services;

public interface ITagService
{
    public Task<ServiceResponse<IEnumerable<string>>> GetTags(string searchText);
}