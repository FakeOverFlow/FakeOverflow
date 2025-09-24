using FakeoverFlow.Backend.Abstraction.Templates;

namespace FakeoverFlow.Backend.Abstraction;

public class EmailTemplates
{
    /// <summary>
    /// Represents the template for the signup verification email.
    /// This email template is intended to send email verification messages containing
    /// dynamic placeholders for user information and verification link.
    /// </summary>
    public static readonly IEmailTemplate SignupVerification = new SignupVerificationEmailTemplate();
}