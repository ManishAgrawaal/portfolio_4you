namespace DevPortfolio.API.Models
{
    public class EmailReply
    {
        public int Id { get; set; }

        public int? ContactMessageId { get; set; }

        public string ResendEmailId { get; set; } = string.Empty;

        public string MessageId { get; set; } = string.Empty;

        public string FromEmail { get; set; } = string.Empty;

        public string ToEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}