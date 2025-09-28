using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Logout;

public static partial class Logout
{
    public class Handler(
        ILogger<Logout.Handler> logger,
        IUserService userService
        ) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Delete("/logout");
            Group<AuthGroup>();
            Description(x =>
            {
                x.WithName("Logout");
                x.Produces<NoContent>(204);
            });
        }

        public async override Task HandleAsync(CancellationToken ct)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error logging out user");
            }
        }
    }
}