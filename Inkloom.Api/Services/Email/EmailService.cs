
using Inkloom.Api.Services.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Inkloom.Api.Services;

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
        await _client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl);
        await _client.AuthenticateAsync(_options.From.Address, _options.Password);
        await _client.SendAsync(message);
        await _client.DisconnectAsync(true);
    }
}

public static class EmailServiceExtensions
{
    public static void AddEmailService<T>(this WebApplicationBuilder builder) where T : EmailService
    {
        builder.Services.AddSingleton<IEmailService, T>().ConfigureSmtpOptions<T>(builder.Configuration);
    }

    public static void ConfigureSmtpOptions<T>(this IServiceCollection services, IConfiguration config) where T : EmailService
    {
        ConfigureSmtpOptions<T>(services, options =>
            {
                var from = config.GetSection("Smtp:From");
                options.From = new(from["Name"], from["Address"]);
                options.Host = config["Smtp:Host"] ?? "";
                var isPortParsed = int.TryParse(config["Smtp:Port"] ?? "", out var port);
                if (isPortParsed)
                {
                    options.Port = port;
                }
                options.UseSsl = config["Smtp:Ssl"] == "true";
                options.Password = config["Smtp:Password"] ?? "";
            });
    }

    public static void ConfigureSmtpOptions<T>(this IServiceCollection services, Action<SmtpOptions> configure) where T : EmailService
    {
        services.Configure(configure);
    }
}