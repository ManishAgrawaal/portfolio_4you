using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace DevPortfolio.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string fromEmail,
            string subject,
            string message)
        {
            var emailSettings =
                _configuration.GetSection("EmailSettings");

            // =========================
            // Read Email Settings
            // =========================

            var username =
                emailSettings["Username"]
                ?? throw new Exception(
                    "EmailSettings:Username is missing."
                );

            var password =
                emailSettings["Password"]
                ?? throw new Exception(
                    "EmailSettings:Password is missing."
                );

            var toEmail =
                emailSettings["ToEmail"]
                ?? throw new Exception(
                    "EmailSettings:ToEmail is missing."
                );

            var smtpServer =
                emailSettings["SmtpServer"]
                ?? throw new Exception(
                    "EmailSettings:SmtpServer is missing."
                );

            var portValue =
                emailSettings["Port"] ?? "587";

            if (!int.TryParse(portValue, out int port))
            {
                port = 587;
            }


            // =========================
            // Create Email
            // =========================

            var email = new MimeMessage();


            // Sender - Your Gmail
            email.From.Add(
                new MailboxAddress(
                    "Portfolio Contact",
                    username
                )
            );


            // Receiver - Your Gmail
            email.To.Add(
                new MailboxAddress(
                    "Portfolio Owner",
                    toEmail
                )
            );


            // Visitor Email
            // When you click Reply,
            // reply will go to visitor's email.
            email.ReplyTo.Add(
                new MailboxAddress(
                    "Visitor",
                    fromEmail
                )
            );


            // Subject
            email.Subject = subject;


            // Message Body
            email.Body = new TextPart("plain")
            {
                Text = message
            };


            // =========================
            // SMTP
            // =========================

            //using var smtp = new SmtpClient();


            //// Local Development Fix
            //// Allows certificate validation issue
            //// on local machine.
            //smtp.ServerCertificateValidationCallback =
            //    (sender, certificate, chain, sslPolicyErrors) => true;


            //// Connect Gmail SMTP
            //await smtp.ConnectAsync(
            //    smtpServer,
            //    port,
            //    MailKit.Security.SecureSocketOptions.StartTls
            //);


            //// Authenticate Gmail
            //await smtp.AuthenticateAsync(
            //    username,
            //    password
            //);


            //// Send Email
            //await smtp.SendAsync(email);


            //// Disconnect
            //await smtp.DisconnectAsync(true);
            // =========================
            // SMTP
            // =========================

            using var smtp = new SmtpClient();

            try
            {
                // Gmail SMTP - Port 587
                await smtp.ConnectAsync(
                    smtpServer,
                    port,
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                // Authenticate Gmail
                await smtp.AuthenticateAsync(
                    username,
                    password
                );

                // Send Email
                await smtp.SendAsync(email);

                // Disconnect
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // This will show the real SMTP error in Render logs
                Console.WriteLine($"SMTP EMAIL ERROR: {ex}");

                if (smtp.IsConnected)
                {
                    await smtp.DisconnectAsync(true);
                }

                throw;
            }
        }
    }
}