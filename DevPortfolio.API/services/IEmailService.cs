//namespace DevPortfolio.API.Services
//{
//    public interface IEmailService
//    {
//        Task SendEmailAsync(
//            string fromEmail,
//            string subject,
//            string message
//        );
//    }
//}
namespace DevPortfolio.API.Services
{
    public interface IEmailService
    {
        // Sends contact notification to Admin
        // Reply-To = Visitor email
        Task SendEmailAsync(
            string fromEmail,
            string subject,
            string message);

        // Sends automatic confirmation to Visitor
        Task SendConfirmationEmailAsync(
            string visitorEmail,
            string visitorName);
    }
}