using System.Collections.Concurrent;
using LMS.Application.Common.Interfaces.Services;

namespace LMS.IntegrationTests.Fakes;

/// <summary>Captures emails instead of sending them, so tests can assert what was sent.</summary>
public class RecordingEmailService : IEmailService
{
    private readonly ConcurrentQueue<EmailMessage> _sent = new();

    public IReadOnlyCollection<EmailMessage> Sent => _sent.ToArray();

    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        _sent.Enqueue(message);
        return Task.CompletedTask;
    }

    public EmailMessage? LastTo(string email) =>
        _sent.LastOrDefault(m => string.Equals(m.To, email, StringComparison.OrdinalIgnoreCase));

    public void Clear() => _sent.Clear();
}
