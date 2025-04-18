using Inkloom.Api.EmailTemplates.OTP;

namespace Inkloom.Api.EmailTemplates;

public class OTPTemplate(string username, string token, string tokenType, string baseUrl) : EmailTemplate<Template>(
    $"{tokenType} OTP",
    new() {
        {"Greeting", $"Hello, {username}"},
        {"OTPMessage", $"Please use this code as {tokenType} OTP you recently requested from inkloom."},
        {"ResetPasswordMessage", $"If you didn't request this code, please reset your password by visiting {baseUrl}/reset-password"},
        {"OTPValue", token}
    })
{ }