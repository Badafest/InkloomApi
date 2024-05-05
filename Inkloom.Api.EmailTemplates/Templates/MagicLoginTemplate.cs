using Inkloom.Api.EmailTemplates.Templates;

namespace Inkloom.Api.EmailTemplates;

public class MagicLoginTemplate(string username, string token) : EmailTemplate<MagicLogin>(new() {
    { "Link", $"https://inkloom.com/magic-login?token={token}" },
    { "Greeting", $"Hello, {username}!" },
    })
{ }