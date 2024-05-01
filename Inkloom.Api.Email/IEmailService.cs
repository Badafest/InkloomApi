using MimeKit;

namespace Inkloom.Api.Email
{
    public class EmailOptions
    {
        public MailboxAddress To { get; set; } = null;
        public string Subject { get; set; } = null;
        public string TextBody { get; set; } = null;
        public string HtmlBody { get; set; } = null;
        public string[] Attachments { get; set; } = null;
    }

    public interface IEmailService
    {
        void SendEmail(EmailOptions options);
    }
}