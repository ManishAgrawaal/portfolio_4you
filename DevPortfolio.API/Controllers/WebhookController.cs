using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPortfolio.API.Data;
using DevPortfolio.API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Mail;

namespace DevPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendWebhook()
        {
            try
            {
                // ==========================================
                // 1. READ WEBHOOK BODY
                // ==========================================

                using var reader = new StreamReader(Request.Body);

                var body = await reader.ReadToEndAsync();

                Console.WriteLine("====================================");
                Console.WriteLine("RESEND WEBHOOK RECEIVED");
                Console.WriteLine(body);
                Console.WriteLine("====================================");


                // ==========================================
                // 2. DESERIALIZE WEBHOOK
                // ==========================================

                var webhook = JsonSerializer.Deserialize<ResendWebhookEvent>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


                if (webhook == null ||
                    webhook.Type != "email.received" ||
                    webhook.Data == null)
                {
                    return Ok();
                }


                var emailId = webhook.Data.EmailId;


                if (string.IsNullOrWhiteSpace(emailId))
                {
                    Console.WriteLine("RESEND EMAIL ID IS MISSING.");

                    return Ok();
                }


                // ==========================================
                // 3. PREVENT DUPLICATE EMAIL
                // ==========================================

                var alreadyExists =
                    await _context.EmailReplies
                        .AnyAsync(x => x.ResendEmailId == emailId);

                if (alreadyExists)
                {
                    Console.WriteLine(
                        "EMAIL ALREADY SAVED: " + emailId);

                    return Ok();
                }


                // ==========================================
                // 4. GET RESEND API KEY
                // ==========================================

                var apiKey =
                    _configuration["EmailSettings:ApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine(
                        "EMAIL API KEY IS MISSING.");

                    return StatusCode(500);
                }


                // ==========================================
                // 5. GET ACTUAL EMAIL FROM RESEND
                // ==========================================

                var client =
                    _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        apiKey);


                var response =
                    await client.GetAsync(
                        $"https://api.resend.com/emails/receiving/{emailId}");


                if (!response.IsSuccessStatusCode)
                {
                    var error =
                        await response.Content.ReadAsStringAsync();

                    Console.WriteLine(
                        "RESEND RECEIVE API ERROR: " + error);

                    return StatusCode(500);
                }


                var responseBody =
                    await response.Content.ReadAsStringAsync();


                var receivedEmail =
                    JsonSerializer.Deserialize<ReceivedEmail>(
                        responseBody,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });


                if (receivedEmail == null)
                {
                    Console.WriteLine(
                        "RECEIVED EMAIL DATA IS NULL.");

                    return Ok();
                }


                // ==========================================
                // 6. EXTRACT VISITOR EMAIL
                // ==========================================

                var visitorEmail =
                    ExtractEmail(receivedEmail.From);


                // ==========================================
                // 7. FIND ORIGINAL CONTACT
                // ==========================================

                var contact =
                    await _context.ContactMessages
                        .Where(x =>
                            x.Email == visitorEmail)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefaultAsync();


                // ==========================================
                // 8. GET MESSAGE CONTENT
                // ==========================================

                var message =
                    !string.IsNullOrWhiteSpace(receivedEmail.Text)
                        ? receivedEmail.Text
                        : receivedEmail.Html;


                // ==========================================
                // 9. SAVE REPLY
                // ==========================================

                var reply = new EmailReply
                {
                    ContactMessageId =
                        contact?.Id,

                    ResendEmailId =
                        receivedEmail.Id,

                    MessageId =
                        receivedEmail.MessageId ?? string.Empty,

                    FromEmail =
                        visitorEmail,

                    ToEmail =
                        receivedEmail.To?.FirstOrDefault()
                        ?? string.Empty,

                    Subject =
                        receivedEmail.Subject
                        ?? string.Empty,

                    Message =
                        message
                        ?? string.Empty,

                    ReceivedAt =
                        receivedEmail.CreatedAt
                };


                _context.EmailReplies.Add(reply);

                await _context.SaveChangesAsync();


                // ==========================================
                // 10. LOG SUCCESS
                // ==========================================

                Console.WriteLine(
                    "VISITOR REPLY SAVED SUCCESSFULLY.");

                Console.WriteLine(
                    "Visitor: " + visitorEmail);

                Console.WriteLine(
                    "Subject: " + receivedEmail.Subject);


                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "RESEND WEBHOOK ERROR:");

                Console.WriteLine(ex);

                return StatusCode(500);
            }
        }


        // =====================================================
        // EXTRACT EMAIL FROM:
        //
        // Name <email@gmail.com>
        //
        // OR
        //
        // email@gmail.com
        // =====================================================

        private static string ExtractEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            try
            {
                var mailAddress =
                    new MailAddress(value);

                return mailAddress.Address;
            }
            catch
            {
                return value.Trim();
            }
        }
    }


    // =========================================================
    // RESEND WEBHOOK MODEL
    // =========================================================

    public class ResendWebhookEvent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public ResendWebhookData? Data { get; set; }
    }


    public class ResendWebhookData
    {
        [JsonPropertyName("email_id")]
        public string EmailId { get; set; } = string.Empty;

        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public List<string>? To { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }
    }


    // =========================================================
    // RESEND RECEIVED EMAIL RESPONSE
    // =========================================================

    public class ReceivedEmail
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("to")]
        public List<string>? To { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("html")]
        public string? Html { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }
    }
}