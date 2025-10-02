using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Abstraction.Templates;
using FakeoverFlow.Backend.Http.Api.Abstracts.Clients;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Events.Models;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Events.Handlers;

public class UserSignupEventHandler(
    ILogger<UserSignupEventHandler> logger,
    IServiceScopeFactory serviceScopeFactory   
    ) : IEventHandler<UserSignupEvent>
{
    public  async Task HandleAsync(UserSignupEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("UserSignupEvent handle started");
        using var scope =  serviceScopeFactory.CreateScope();
        var userService = scope.Resolve<IUserService>();
        var emailClient = scope.Resolve<IEmailClient>();
        var configuration = scope.Resolve<IConfiguration>();
        
        var userAccount = await userService.GetOrCreateUserVerificationAsync(eventModel.UserId);
        if (userAccount.IsFailure || userAccount.Value is null)
        {
            logger.LogWarning("UserSignupEvent handle failed, user not found");
            return;
        }
        var value = userAccount.Value!;
        
        if(value.Account.VerifiedOn is not null)
        {
            logger.LogWarning("UserSignupEvent handle failed, user already verified");
            return;
        }
        
        var baseDomain = configuration.GetValue<string>("Frontend:FrontendBaseUrl") ?? string.Empty;
        var verificationLink = $"{baseDomain}/auth/verify/{value.VerificationToken}";
        var userName = value.Account.Username;
        logger.LogInformation("Verification Link: {VerificationLink}", verificationLink);
        logger.LogInformation("User Name: {UserName}", userName);
        Dictionary<string, string> placeholders = new()
        {
            { SignupVerificationEmailTemplate.UserNameKey, userName },
            { SignupVerificationEmailTemplate.VerificationLinkKey, verificationLink }
        };
        
        var emailSendResponse = await emailClient.SendEmailAsync(EmailTemplates.SignupVerification, [value.Account.Email], null, null, placeholders);
        if(!emailSendResponse)
        {
            logger.LogWarning("UserSignupEvent handle failed, email send failed");
            return;
        }

        logger.LogInformation("UserSignupEvent handle completed");
    }
}