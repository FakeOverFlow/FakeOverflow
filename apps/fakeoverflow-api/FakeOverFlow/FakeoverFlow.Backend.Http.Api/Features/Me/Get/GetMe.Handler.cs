using System.Security.Claims;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Me.Get;

public static partial class GetMe
{
    public class Handler(
        IUserService userService,
        ILogger<Handler> logger
        ) : EndpointWithoutRequest<Results<Ok<Response>, ErrorResponse, ProblemDetails>>
    {
        public override void Configure()
        {
            Get("");
            Group<MeGroup>();
            Description(x =>
            {
                x.WithName("me");
                x.Produces<Response>(200);
                x.Produces<ProblemDetails>(401);
                x.Produces<ProblemDetails>(500);
                x.Produces<ProblemDetails>(404);
            });
        }

        public override async Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(CancellationToken ct)
        {
            var nameIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdClaim is null)
            {
                AddError(($"User is not authenticated"));
                ThrowIfAnyErrors();
            }
            
            if (!Guid.TryParse(nameIdClaim!.Value, out var userId))
            {
                AddError(($"User is not authenticated"));
                ThrowIfAnyErrors();
            }
            
            logger.LogInformation("User {UserId} is getting their profile", userId);
            var user = await userService.GetUserByIdAsync(userId, track: false, cancellationToken: ct);
            
            if(user is null)
                return new ErrorResponse()
                {
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound,
                };

            return TypedResults.Ok(new Response()
            {
                Id = user.Id.ToString(),
                UserName = user.Username,
                ProfilePicture = user.ProfileImageUrl,
                Email = user.Email,
                Settings = new MeResponseSettings()
                {

                }
            });
        }
    }
}