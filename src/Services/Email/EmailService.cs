
using InkloomApi.Services.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InkloomApi.Services;

public class EmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpClient _client = new();

    private readonly SmtpOptions _options = options.Value;
    private MimeMessage GetMimeMessage(EmailOptions options)
    {
        var message = new MimeMessage
        {
            Subject = options.Subject
        };
        message.From.Add(_options.From);
        message.To.Add(options.To);

        var builder = new BodyBuilder
        {
            TextBody = options.TextBody,
            HtmlBody = options.HtmlBody
        };

        if (options.Attachments != null)
        {
            foreach (var attachment in options.Attachments)
            {
                builder.Attachments.Add(attachment);
            }
        }

        message.Body = builder.ToMessageBody();

        return message;

    }
    public async void SendEmail(EmailOptions options)
    {
        var message = GetMimeMessage(options);
        var cancellationToken = new CancellationToken();
        await _client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);
        await _client.AuthenticateAsync(_options.From.Address, _options.Password);
        await _client.SendAsync(message);
        await _client.DisconnectAsync(true);
    }
}