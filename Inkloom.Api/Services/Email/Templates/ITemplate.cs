namespace Inkloom.Api.Services.Email.Templates;

public interface ITemplate
{
    string GetHtmlBody();

    string GetTextBody();
}