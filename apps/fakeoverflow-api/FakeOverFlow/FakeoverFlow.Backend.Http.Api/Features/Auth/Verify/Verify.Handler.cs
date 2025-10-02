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
        public override async Task HandleAsync(CancellationToken ct)
        {
            if (!HttpContext.Request.RouteValues.TryGetValue("id", out var id) || id == null)
            {
                logger.LogCritical("This should not happen, but failed to get the id from the route");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            
            var tokenId = id.ToString();
            if(string.IsNullOrWhiteSpace(tokenId))
            {
                logger.LogCritical("This should not happen, but failed to get the id from the route");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            var result = await userService.VerifyAccountAsync(tokenId);
            if (result.IsFailure)
            {
                logger.LogWarning("Verify failed due to {VerifyFailedReason}", result.Error!.Detail);
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            return;
        }

        public override void Configure()
        {
            Description((x) =>
            {
                x.WithName("verify");
                x.WithDescription("Verify the user");
            });
            Post("/verify/{id}");
            Group<AuthGroup>();
            AllowAnonymous();
        }
    }
    
}