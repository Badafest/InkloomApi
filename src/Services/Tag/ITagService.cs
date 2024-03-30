namespace InkloomApi.Services;

public interface ITagService
{
    public Task<ServiceResponse<IEnumerable<string>>> GetTags(string searchText);
}