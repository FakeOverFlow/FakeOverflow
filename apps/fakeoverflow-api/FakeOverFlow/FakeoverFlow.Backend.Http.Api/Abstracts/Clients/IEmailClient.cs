using FakeoverFlow.Backend.Abstraction;

namespace FakeoverFlow.Backend.Http.Api.Abstracts.Clients;

public interface IEmailClient
{
    /// <summary>
    /// Sends an email with the specified subject, message, and recipient information.
    /// </summary>
    /// <param name="message">The body content of the email.</param>
    /// <param name="toList">An array of recipient email addresses to send the email to.</param>
    /// <param name="ccList">An array of recipient email addresses to include as CC.</param>
    /// <param name="bccList">An array of recipient email addresses to include as BCC.</param>
    /// <param name="placeholders">A dictionary of placeholder values to replace within the email content.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the email was successfully sent.</returns>
    Task<bool> SendEmailAsync(
        IEmailTemplate message,
        string[]? toList = null,
        string[]? ccList = null,
        string[]? bccList = null,
        Dictionary<string, string>? placeholders = null
    );
}

public class NullEmailClient(ILogger<NullEmailClient> logger, IWebHostEnvironment webHost)
    : IEmailClient
{
    public Task<bool> SendEmailAsync(IEmailTemplate message, string[]? toList = null, string[]? ccList = null,
        string[]? bccList = null, Dictionary<string, string>? placeholders = null)
    {
        logger.LogInformation("Mocking email sending.");
        if (placeholders != null)
        {
            foreach (var placeholdersKey in placeholders.Keys)
            {
                logger.LogInformation("Placeholder {Placeholder} replaced with {Value}", placeholdersKey, webHost.IsDevelopment() ? placeholders[placeholdersKey] : "*****");
            }
        }
        return Task.FromResult(true);
    }
}