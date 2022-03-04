using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Web_App.Services.Contracts;
using Web_App.Settings;

namespace Web_App.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSetting> smtpSetting;

        public EmailService(IOptions<SmtpSetting> smtpSetting)
        {
            this.smtpSetting = smtpSetting;
        }
        public async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);
            message.IsBodyHtml = true;
            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(smtpSetting.Value.User, smtpSetting.Value.Password);
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
