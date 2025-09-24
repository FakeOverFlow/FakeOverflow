using FastEndpoints;
using FluentValidation;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Login;

public static partial class Login
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            When(x => x.Type == AuthenticationType.Credentials, () =>
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .WithMessage("Username is required");
                
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password is required");
            });

            When(x => x.Type == AuthenticationType.Google, () =>
            {
                RuleFor(x => x.Code)
                    .NotEmpty()
                    .MustAsync((code, cancellationToken) => Task.FromResult(true));
            });
        }
    }
}