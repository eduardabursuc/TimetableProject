using Domain.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            
            var smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = int.Parse(configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
                EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"]),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(configuration["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}