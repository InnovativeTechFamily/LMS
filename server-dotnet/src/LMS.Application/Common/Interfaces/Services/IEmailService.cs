namespace LMS.Application.Common.Interfaces.Services;

/// <summary>Renders an EJS-equivalent template and sends it over SMTP.</summary>
public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}

/// <param name="Template">Template file name without extension, e.g. "activation-mail".</param>
public record EmailMessage(string To, string Subject, string Template, IDictionary<string, object?> Data);
