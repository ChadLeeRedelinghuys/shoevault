using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ShoeVault.Services
{


    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine("EMAIL FUNCTION TRIGGERED");

            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]!),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:From"]!, "ShoeVault Security"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}