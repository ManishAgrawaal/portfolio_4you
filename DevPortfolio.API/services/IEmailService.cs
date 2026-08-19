namespace DevPortfolio.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string fromEmail,
            string subject,
            string message
        );
    }
}