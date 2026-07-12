namespace LMS.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string email, string name, string activationCode);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendNotificationEmailAsync(string email, string subject, string htmlContent);
        Task SendOrderConfirmationEmailAsync(string email, string orderDetails);
    }
}
