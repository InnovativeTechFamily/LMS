using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LMS.Infrastructure.Email;

/// <summary>Renders an HTML template and delivers it over SMTP (MailKit), replacing Nodemailer.</summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly string _templateRoot;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _templateRoot = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            _logger.LogWarning("SMTP host is not configured; skipping email '{Subject}' to {To}.",
                message.Subject, message.To);
            return;
        }

        var body = await RenderAsync(message.Template, message.Data, ct);

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;
        email.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.Auto, ct);
        if (!string.IsNullOrEmpty(_settings.User))
            await client.AuthenticateAsync(_settings.User, _settings.Password, ct);
        await client.SendAsync(email, ct);
        await client.DisconnectAsync(true, ct);
    }

    private async Task<string> RenderAsync(string templateName, IDictionary<string, object?> data, CancellationToken ct)
    {
        var path = Path.Combine(_templateRoot, $"{templateName}.html");
        if (!File.Exists(path))
        {
            _logger.LogWarning("Email template '{Template}' not found at {Path}; sending raw values.", templateName, path);
            return string.Join("<br/>", data.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        var template = await File.ReadAllTextAsync(path, ct);
        return EmailTemplateRenderer.Render(template, data);
    }
}
