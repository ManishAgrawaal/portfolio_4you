public interface IEmailService
{
    Task SendEmailAsync(
        string fromEmail,
        string subject,
        string message
    );

    Task SendConfirmationEmailAsync(
        string visitorEmail,
        string visitorName
    );

    Task<string> SendReplyAsync(
        string toEmail,
        string subject,
        string message,
        string inReplyTo,
        List<string> references
    );
}