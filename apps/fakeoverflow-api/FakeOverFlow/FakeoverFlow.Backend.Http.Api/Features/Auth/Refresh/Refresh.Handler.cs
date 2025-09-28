using System.Security.Claims;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FakeoverFlow.Backend.Http.Api.Options;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Refresh;

internal partial class Refresh
{
    public class Handler(
        IConfiguration configuration,
        IUserService userService,
        ILogger<Refresh> logger
        ) : Endpoint<Request, Results<Ok<Response>, ErrorResponse, ProblemDetails>>
    {
        public override async Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            var reqAccessToken = req.AccessToken;
            // Get the user id from the token
            if (!HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return new ErrorResponse()
                {
                    Message = "User is not authenticated",
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }

            var jti = HttpContext.User.FindFirst("jti");
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (jti is null || userId is null)
                return new ErrorResponse()
                {
                    Message = "User is not authenticated",
                    StatusCode = StatusCodes.Status401Unauthorized,
                };

            var jtiValue = jti.Value;
            var rawUserIdValue = userId.Value;
            if (Guid.TryParse(rawUserIdValue, out var userIdValue))
            {
                return new ErrorResponse()
                {
                    Message = "User is not authenticated",
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }

            var oldRefreshToken  = await userService.GetRefreshTokenOf(userIdValue, jtiValue, checkExpiry: true, checkRevoked: true, cancellationToken: ct);
            if (oldRefreshToken.IsFailure)
                return oldRefreshToken.Error!.ToFastEndpointError();
            
            var tokenId = Ulid.NewUlid().ToString();
            var jwtOptions = configuration.GetSection("JwtSettings").Get<JwtOptions>();
            if (jwtOptions is null)
                return new ErrorResponse()
                {
                    Message = "JwtSettings not configured",
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            
            var refreshToken = await userService.CreateRefreshTokenForUserAsync(userId: oldRefreshToken.Value!.UserId, jti: tokenId, cancellationToken: ct);
            if (refreshToken.IsFailure)
                return refreshToken.Error!.ToFastEndpointError();
            
            
            var tokenExpiryOn = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes);
            var token = JwtBearer.CreateToken(refreshToken.Value!.Account.JwtCreationOptions(jwtOptions, tokenExpiryOn, tokenId));

            return TypedResults.Ok(new Response()
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiryOn,
                RefreshToken = refreshToken.Value.Id,
                RefreshTokenExpires = refreshToken.Value.ExpiresOn,
            });
        }

        public override void Configure()
        {
            Post("/refresh");
            Group<AuthGroup>();
            Description(x =>
            {
                x.WithName("Refresh");
                x.Produces<Ok<Response>>(200);
                x.Produces<ErrorResponse>(400);
            });
        }
    }
}