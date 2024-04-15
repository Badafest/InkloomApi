using MimeKit;

namespace InkloomApi.Services;

public class EmailOptions
{
    public MailboxAddress To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string TextBody { get; set; } = null!;
    public string? HtmlBody { get; set; } = null;
    public string[]? Attachments { get; set; } = null;
}

public interface IEmailService
{
    Task SendEmail(EmailOptions options);
}