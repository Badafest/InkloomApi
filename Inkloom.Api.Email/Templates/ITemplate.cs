namespace Inkloom.Api.Email.Templates
{
    public interface ITemplate
    {
        string GetHtmlBody();

        string GetTextBody();
    }
}