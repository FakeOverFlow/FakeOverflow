using FastEndpoints;
using FluentValidation;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;

public static partial class Signup
{

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required");
            
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Username must be less than 50 characters");
            
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .Must(x => x.Any(char.IsUpper) && x.Any(char.IsLower) && x.Any(char.IsDigit))
                .WithMessage("Password must contain at least one uppercase, one lowercase, and one digit");
        }
    }
    
}