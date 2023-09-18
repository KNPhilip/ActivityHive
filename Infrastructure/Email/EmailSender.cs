using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string userEmail, string emailSubject, string msg)
        {
            SendGridClient client = new(_config["Sendgrid:Key"]);

            SendGridMessage message = new()
            {
                From = new EmailAddress("knphilip@outlook.com", _config["Sendgrid:User"]),
                Subject = emailSubject,
                PlainTextContent = msg,
                HtmlContent = msg
            };
            message.AddTo(new EmailAddress(userEmail));
            message.SetClickTracking(false, false);

            await client.SendEmailAsync(message);
        }
    }
}