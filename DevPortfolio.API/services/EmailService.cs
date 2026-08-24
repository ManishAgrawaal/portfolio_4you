////using MailKit.Net.Smtp;
////using Microsoft.Extensions.Configuration;
////using MimeKit;

////namespace DevPortfolio.API.Services
////{
////    public class EmailService : IEmailService
////    {
////        private readonly IConfiguration _configuration;

////        public EmailService(IConfiguration configuration)
////        {
////            _configuration = configuration;
////        }

////        public async Task SendEmailAsync(
////            string fromEmail,
////            string subject,
////            string message)
////        {
////            var emailSettings =
////                _configuration.GetSection("EmailSettings");

////            // =========================
////            // Read Email Settings
////            // =========================

////            var username =
////                emailSettings["Username"]
////                ?? throw new Exception(
////                    "EmailSettings:Username is missing."
////                );

////            var password =
////                emailSettings["Password"]
////                ?? throw new Exception(
////                    "EmailSettings:Password is missing."
////                );

////            var toEmail =
////                emailSettings["ToEmail"]
////                ?? throw new Exception(
////                    "EmailSettings:ToEmail is missing."
////                );

////            var smtpServer =
////                emailSettings["SmtpServer"]
////                ?? throw new Exception(
////                    "EmailSettings:SmtpServer is missing."
////                );

////            var portValue =
////                emailSettings["Port"] ?? "587";

////            if (!int.TryParse(portValue, out int port))
////            {
////                port = 587;
////            }


////            // =========================
////            // Create Email
////            // =========================

////            var email = new MimeMessage();


////            // Sender - Your Gmail
////            email.From.Add(
////                new MailboxAddress(
////                    "Portfolio Contact",
////                    username
////                )
////            );


////            // Receiver - Your Gmail
////            email.To.Add(
////                new MailboxAddress(
////                    "Portfolio Owner",
////                    toEmail
////                )
////            );


////            // Visitor Email
////            // When you click Reply,
////            // reply will go to visitor's email.
////            email.ReplyTo.Add(
////                new MailboxAddress(
////                    "Visitor",
////                    fromEmail
////                )
////            );


////            // Subject
////            email.Subject = subject;


////            // Message Body
////            email.Body = new TextPart("plain")
////            {
////                Text = message
////            };


////            // =========================
////            // SMTP
////            // =========================

////            //using var smtp = new SmtpClient();


////            //// Local Development Fix
////            //// Allows certificate validation issue
////            //// on local machine.
////            //smtp.ServerCertificateValidationCallback =
////            //    (sender, certificate, chain, sslPolicyErrors) => true;


////            //// Connect Gmail SMTP
////            //await smtp.ConnectAsync(
////            //    smtpServer,
////            //    port,
////            //    MailKit.Security.SecureSocketOptions.StartTls
////            //);


////            //// Authenticate Gmail
////            //await smtp.AuthenticateAsync(
////            //    username,
////            //    password
////            //);


////            //// Send Email
////            //await smtp.SendAsync(email);


////            //// Disconnect
////            //await smtp.DisconnectAsync(true);
////            // =========================
////            // SMTP
////            // =========================

////            using var smtp = new SmtpClient();

////            try
////            {
////                // Gmail SMTP - Port 587
////                await smtp.ConnectAsync(
////                    smtpServer,
////                    port,
////                    MailKit.Security.SecureSocketOptions.StartTls
////                );

////                // Authenticate Gmail
////                await smtp.AuthenticateAsync(
////                    username,
////                    password
////                );

////                // Send Email
////                await smtp.SendAsync(email);

////                // Disconnect
////                await smtp.DisconnectAsync(true);
////            }
////            catch (Exception ex)
////            {
////                // This will show the real SMTP error in Render logs
////                Console.WriteLine($"SMTP EMAIL ERROR: {ex}");

////                if (smtp.IsConnected)
////                {
////                    await smtp.DisconnectAsync(true);
////                }

////                throw;
////            }
////        }
////    }
////}
//using Microsoft.Extensions.Configuration;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace DevPortfolio.API.Services
//{
//    public class EmailService : IEmailService
//    {
//        private static readonly HttpClient _httpClient = new HttpClient();

//        private readonly IConfiguration _configuration;

//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task SendEmailAsync(
//            string fromEmail,
//            string subject,
//            string message)
//        {
//            var emailSettings =
//                _configuration.GetSection("EmailSettings");

//            // =========================
//            // Read Email Settings
//            // =========================

//            var apiKey =
//                emailSettings["ApiKey"]
//                ?? throw new Exception(
//                    "EmailSettings:ApiKey is missing.");

//            var fromAddress =
//                emailSettings["FromEmail"]
//                ?? throw new Exception(
//                    "EmailSettings:FromEmail is missing.");

//            var toEmail =
//                emailSettings["ToEmail"]
//                ?? throw new Exception(
//                    "EmailSettings:ToEmail is missing.");

//            // =========================
//            // Resend HTTP API
//            // =========================

//            var payload = new
//            {
//                from = fromAddress,
//                to = new[] { toEmail },
//                subject = subject,
//                text = message,
//                reply_to = fromEmail
//            };

//            var json = JsonSerializer.Serialize(payload);

//            using var request = new HttpRequestMessage(
//                HttpMethod.Post,
//                "https://api.resend.com/emails");

//            request.Headers.Authorization =
//                new AuthenticationHeaderValue("Bearer", apiKey);

//            request.Content = new StringContent(
//                json,
//                Encoding.UTF8,
//                "application/json");

//            try
//            {
//                using var response =
//                    await _httpClient.SendAsync(request);

//                var responseBody =
//                    await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine(
//                        $"RESEND EMAIL ERROR: {(int)response.StatusCode} {response.StatusCode}");

//                    Console.WriteLine(
//                        $"RESEND RESPONSE: {responseBody}");

//                    throw new Exception(
//                        $"Email service failed: {response.StatusCode}");
//                }

//                Console.WriteLine(
//                    $"EMAIL SENT SUCCESSFULLY: {responseBody}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(
//                    $"EMAIL API ERROR: {ex}");

//                throw;
//            }
//        }
//    }
//}

using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DevPortfolio.API.Services
{
    public class EmailService : IEmailService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =====================================================
        // 1. SEND ADMIN NOTIFICATION
        // =====================================================

        public async Task SendEmailAsync(
            string visitorEmail,
            string subject,
            string message)
        {
            var emailSettings =
                _configuration.GetSection("EmailSettings");

            // =========================
            // Read Configuration
            // =========================

            var apiKey =
                emailSettings["ApiKey"]
                ?? throw new Exception(
                    "EmailSettings:ApiKey is missing.");

            var fromAddress =
                emailSettings["FromEmail"]
                ?? throw new Exception(
                    "EmailSettings:FromEmail is missing.");

            var toEmail =
                emailSettings["ToEmail"]
                ?? throw new Exception(
                    "EmailSettings:ToEmail is missing.");

            // =========================
            // Admin Email
            // =========================

            var payload = new
            {
                from = fromAddress,

                // Admin receives the contact message
                to = new[]
                {
                    toEmail
                },

                subject = subject,

                text = message,

                // IMPORTANT:
                // When Admin clicks Reply,
                // reply goes directly to Visitor.
                reply_to = visitorEmail
            };

            await SendResendEmailAsync(
                apiKey,
                payload,
                "ADMIN EMAIL");
        }


        // =====================================================
        // 2. SEND VISITOR CONFIRMATION
        // =====================================================

        public async Task SendConfirmationEmailAsync(
            string visitorEmail,
            string visitorName)
        {
            var emailSettings =
                _configuration.GetSection("EmailSettings");

            // =========================
            // Read Configuration
            // =========================

            var apiKey =
                emailSettings["ApiKey"]
                ?? throw new Exception(
                    "EmailSettings:ApiKey is missing.");

            var fromAddress =
                emailSettings["FromEmail"]
                ?? throw new Exception(
                    "EmailSettings:FromEmail is missing.");

            // =========================
            // Confirmation Subject
            // =========================

            var subject =
                "Thank you for contacting Manish Technology Solution";

            // =========================
            // Confirmation Message
            // =========================

            var message =
$@"Hi {visitorName},

Thank you for contacting Manish Technology Solution.

We have received your message successfully.

Our team will review your message and get back to you soon.

Regards,
Manish Technology Solution

https://manishtechnologysolution.com";


            // =========================
            // Visitor Email
            // =========================

            var payload = new
            {
                from = fromAddress,

                // Confirmation goes to Visitor
                to = new[]
                {
                    visitorEmail
                },

                subject = subject,

                text = message,

                // If Visitor replies to confirmation,
                // reply will go to Admin.
                reply_to = fromAddress
            };

            await SendResendEmailAsync(
                apiKey,
                payload,
                "VISITOR CONFIRMATION EMAIL");
        }


        // =====================================================
        // 3. COMMON RESEND METHOD
        // =====================================================

        private static async Task SendResendEmailAsync(
            string apiKey,
            object payload,
            string emailType)
        {
            var json =
                JsonSerializer.Serialize(payload);

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.resend.com/emails");

            // =========================
            // Resend Authorization
            // =========================

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            // =========================
            // Request Body
            // =========================

            request.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            try
            {
                using var response =
                    await _httpClient.SendAsync(request);

                var responseBody =
                    await response.Content.ReadAsStringAsync();

                // =========================
                // Check Response
                // =========================

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        $"{emailType} ERROR: " +
                        $"{(int)response.StatusCode} " +
                        $"{response.StatusCode}");

                    Console.WriteLine(
                        $"RESEND RESPONSE: " +
                        $"{responseBody}");

                    throw new Exception(
                        $"{emailType} failed: " +
                        $"{response.StatusCode}");
                }

                // =========================
                // Success
                // =========================

                Console.WriteLine(
                    $"{emailType} SENT SUCCESSFULLY: " +
                    $"{responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{emailType} API ERROR: {ex}");

                throw;
            }
        }
    }
}