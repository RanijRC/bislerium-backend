using Bislerium.Infrastructure.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Repository.Implementation
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var smtpServer = configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            var smtpUsername = configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = configuration["EmailSettings:SmtpPassword"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true; // Enable SSL/TLS
                client.UseDefaultCredentials = false; // Do not use default credentials
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword); // Set SMTP credentials

                var message = new MailMessage(smtpUsername, email, subject, body);
                await client.SendMailAsync(message);
            }
        }
    }
}
