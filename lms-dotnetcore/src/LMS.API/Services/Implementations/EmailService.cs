using LMS.API.Configuration;
using LMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LMS.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendActivationEmailAsync(string email, string name, string activationCode)
        {
            _logger.LogInformation("Sending activation email to: {Email}", email);

            var subject = "Activate Your LMS Account";
            var htmlContent = $@"
                <html>
                    <body>
                        <h1>Welcome to LMS!</h1>
                        <p>Hi {name},</p>
                        <p>Thank you for registering. Please use the activation code below to verify your email address:</p>
                        <h2>{activationCode}</h2>
                        <p>This code will expire in 5 minutes.</p>
                        <p>Best regards,<br/>LMS Team</p>
                    </body>
                </html>
            ";

            await SendEmailAsync(email, subject, htmlContent);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            _logger.LogInformation("Sending password reset email to: {Email}", email);

            var subject = "Reset Your LMS Password";
            var htmlContent = $@"
                <html>
                    <body>
                        <h1>Password Reset Request</h1>
                        <p>You requested to reset your password. Click the link below:</p>
                        <a href=""{resetLink}"">Reset Password</a>
                        <p>This link will expire in 1 hour.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                        <p>Best regards,<br/>LMS Team</p>
                    </body>
                </html>
            ";

            await SendEmailAsync(email, subject, htmlContent);
        }

        public async Task SendNotificationEmailAsync(string email, string subject, string htmlContent)
        {
            _logger.LogInformation("Sending notification email to: {Email}", email);
            await SendEmailAsync(email, subject, htmlContent);
        }

        public async Task SendOrderConfirmationEmailAsync(string email, string orderDetails)
        {
            _logger.LogInformation("Sending order confirmation email to: {Email}", email);

            var subject = "Order Confirmation - LMS";
            var htmlContent = $@"
                <html>
                    <body>
                        <h1>Order Confirmed!</h1>
                        <p>Thank you for your purchase!</p>
                        <p>Order Details:</p>
                        <pre>{orderDetails}</pre>
                        <p>You can now access the course materials.</p>
                        <p>Best regards,<br/>LMS Team</p>
                    </body>
                </html>
            ";

            await SendEmailAsync(email, subject, htmlContent);
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string htmlContent)
        {
            try
            {
                var client = new SendGridClient(_emailSettings.SendGridKey);
                var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                var to = new EmailAddress(recipientEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send email. Status: {StatusCode}", response.StatusCode);
                    throw new InvalidOperationException($"Failed to send email. Status: {response.StatusCode}");
                }

                _logger.LogInformation("Email sent successfully to: {Email}", recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to: {Email}", recipientEmail);
                throw;
            }
        }
    }
}
