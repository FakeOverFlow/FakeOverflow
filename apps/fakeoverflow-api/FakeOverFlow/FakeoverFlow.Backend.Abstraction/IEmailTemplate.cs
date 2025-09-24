namespace FakeoverFlow.Backend.Abstraction;
public interface IEmailTemplate
{
    /// <summary>
    /// Returns the email subject line for the signup verification template.
    /// </summary>
    /// <param name="parameters">Optional parameters used to construct the subject line.</param>
    /// <returns>The subject line as a string.</returns>
    string asSubject(Dictionary<string, string>? parameters);


    /// <summary>
    /// Generates an email template with placeholders replaced by the provided parameter values.
    /// </summary>
    /// <param name="parameters">
    /// A dictionary containing keys and values for placeholders within the email template.
    /// Common keys include "VerificationLink" and "UserName".
    /// </param>
    /// <returns>
    /// A string containing the formatted email template with parameterized content.
    /// </returns>
    string AsMailTemplate(Dictionary<string, string>? parameters);
}