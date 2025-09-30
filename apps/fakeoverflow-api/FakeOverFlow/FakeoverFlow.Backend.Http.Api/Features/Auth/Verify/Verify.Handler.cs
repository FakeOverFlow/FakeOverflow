using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Verify;

public static partial class Verify
{
    
    public class Handler(
        ILogger<Verify.Handler> logger,
        IUserService userService
        ) : EndpointWithoutRequest
    {
        public override Task HandleAsync(CancellationToken ct)
        {
            return base.HandleAsync(ct);
        }

        public override void Configure()
        {
            Description((x) =>
            {
                x.WithName("verify");
                x.WithDescription("Verify the user");
            });
            Post("/verify");
            Group<AuthGroup>();
            AllowAnonymous();
        }
    }
    
}