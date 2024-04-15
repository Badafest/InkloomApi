using MimeKit;

namespace InkloomApi.Services.Email;

public class SmtpOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 465;
    public bool UseSsl { get; set; } = true;
    public MailboxAddress From { get; set; } = null!;

    public string Password { get; set; } = null!;
}