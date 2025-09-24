namespace FakeoverFlow.Backend.Abstraction.Templates;

public class SignupVerificationEmailTemplate : IEmailTemplate
{
    /// <summary>
    /// A key used to identify the verification link within the parameters
    /// of the email template. This key is substituted with the actual
    /// verification URL to be included in the email content.
    /// </summary>
    public const string VerificationLinkKey = "VerificationLink";

    /// <summary>
    /// A key used to identify the username within the parameters
    /// of the email template. This key is substituted with the actual
    /// username to personalize the email content.
    /// </summary>
    public const string UserNameKey = "UserName";

    public string asSubject(Dictionary<string, string> parameters)
    {
        return """
               Hey solider! Verify your email address
               """;
    }

    /// <summary>
    /// Generates an email template as a string using the specified parameters.
    /// </summary>
    /// <param name="parameters">
    /// A dictionary containing key-value pairs for replacing placeholders in the template.
    /// Expected keys include "VerificationLink" and "UserName".
    /// </param>
    /// <returns>
    /// A string containing the email template with placeholders replaced by the provided parameter values.
    /// </returns>
    public string AsMailTemplate(Dictionary<string, string> parameters)
    {
      return $"""
              <!DOCTYPE html>
              <html>
              <head>
                <meta charset="UTF-8">
                <title>Email Verification</title>
                <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
              </head>
              <body style="margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f4f4;">
                <table role="presentation" cellpadding="0" cellspacing="0" width="100%">
                  <tr>
                    <td align="center" style="padding:20px 0;">
                      <table role="presentation" cellpadding="0" cellspacing="0" width="100%" style="max-width:600px; background:#ffffff; border-radius:8px; overflow:hidden;">
                        <tr>
                          <td align="center" style="padding:30px;">
                            <h2 style="margin:0 0 20px; font-size:24px; color:#333;">Welcome, {parameters.GetValueOrDefault(UserNameKey, "there...")}</h2>
                            <p style="margin:0 0 20px; font-size:16px; color:#555;">
                              Please verify your email by clicking the button below:
                            </p>
                            <a href="{parameters.GetValueOrDefault(VerificationLinkKey, string.Empty)}" target="_blank" 
                               style="display:inline-block; padding:12px 24px; font-size:16px; color:#ffffff; background:#007bff; border-radius:4px; text-decoration:none;">
                              Verify Email
                            </a>
                            <p style="margin-top:20px; font-size:14px; color:#888;">
                              If you didn’t create an account, you can safely ignore this email.
                            </p>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
              </html>
              """;
      }
}