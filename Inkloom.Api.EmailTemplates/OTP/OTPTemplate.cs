using Inkloom.Api.EmailTemplates.OTP;

namespace Inkloom.Api.EmailTemplates;

public class OTPTemplate(string username, string token, string tokenType) : EmailTemplate<Template>(
    new() {
        {"Greeting", $"Hello, {username}"},
        {"OTPMessage", $"Please use this code as {tokenType} OTP you recently requested from inkloom."},
        {"OTPValue", token}
    })
{ }