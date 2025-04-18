using Inkloom.Api.EmailTemplates.MagicLogin;

namespace Inkloom.Api.EmailTemplates;

public class MagicLoginTemplate(string username, string baseUrl, string token) : EmailTemplate<Template>(
    "Magic Login",
    new() {
        { "Greeting", $"Hello, {username}" },
        { "Link", $"{baseUrl}/magic-login?token={token}" }
    })
{ }