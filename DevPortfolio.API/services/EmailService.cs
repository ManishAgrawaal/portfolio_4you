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
        public async Task<string> SendReplyAsync(
    string toEmail,
    string subject,
    string message,
    string inReplyTo,
    List<string> references)
        {
            var emailSettings =
                _configuration.GetSection("EmailSettings");

            var apiKey =
                emailSettings["ApiKey"]
                ?? throw new Exception(
                    "EmailSettings:ApiKey is missing.");

            var fromAddress =
                emailSettings["FromEmail"]
                ?? throw new Exception(
                    "EmailSettings:FromEmail is missing.");

            var payload = new
            {
                from = fromAddress,

                to = new[]
                {
            toEmail
        },

                subject = subject,

                text = message,

                headers = new Dictionary<string, string>
                {
                    ["In-Reply-To"] = inReplyTo,

                    ["References"] =
                        string.Join(" ", references)
                }
            };

            var json =
                JsonSerializer.Serialize(payload);

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.resend.com/emails");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            request.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            using var response =
                await _httpClient.SendAsync(request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"RESEND REPLY ERROR: {(int)response.StatusCode}");

                Console.WriteLine(
                    $"RESEND RESPONSE: {responseBody}");

                throw new Exception(
                    $"Reply email failed: {response.StatusCode}");
            }

            Console.WriteLine(
                $"ADMIN REPLY SENT: {responseBody}");

            using var responseJson =
                JsonDocument.Parse(responseBody);

            var emailId =
                responseJson.RootElement
                    .GetProperty("id")
                    .GetString();

            return emailId ?? string.Empty;
        }
    }
}