using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Net;

namespace Identity_Infrastructure.Configurations.EmailConfiguration
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string ccEmail = null, string bccEmail = null)
        {
            // Fetch email settings from configuration
            var smtpServer = _configuration["EmailSettings:ServerAddress"];
            var smtpPort = int.Parse(_configuration["EmailSettings:ServerPort"]);
            var enableSsl = bool.Parse(_configuration["EmailSettings:ServerUseSsl"]);
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var fromAddress = _configuration["EmailSettings:FromAddress"];
            var fromName = _configuration["EmailSettings:FromName"];

            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = enableSsl;

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if sending HTML content
                };

                // Add recipient(s)
                mailMessage.To.Add(toEmail);

                // Add CC if provided
                if (!string.IsNullOrEmpty(ccEmail))
                    mailMessage.CC.Add(ccEmail);

                // Add BCC if provided
                if (!string.IsNullOrEmpty(bccEmail))
                    mailMessage.Bcc.Add(bccEmail);

                // Send the email asynchronously
                await smtpClient.SendMailAsync(mailMessage);
                
            }
        }
    }
}
