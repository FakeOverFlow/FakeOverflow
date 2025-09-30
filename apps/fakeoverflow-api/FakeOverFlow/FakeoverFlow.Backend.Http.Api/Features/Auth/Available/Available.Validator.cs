using FastEndpoints;
using FluentValidation;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Available;

public static partial class Available
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            When(x => x.Type == AvailabilityType.Email, () =>
            {
                RuleFor(x => x.Value)
                    .NotEmpty()
                    .EmailAddress();
            });
            
            RuleFor(x => x.Value)
                .NotEmpty()
                .MinimumLength(3);
        }
    }
}