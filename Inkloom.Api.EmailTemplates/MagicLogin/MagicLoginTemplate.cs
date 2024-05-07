using Inkloom.Api.EmailTemplates.MagicLogin;

namespace Inkloom.Api.EmailTemplates;

public class MagicLoginTemplate(string username, string token) : EmailTemplate<Template>(
    new() {
        { "Greeting", $"Hello, {username}" },
        { "Link", $"https://inkloom.com/magic-login?token={token}" }
    })
{ }