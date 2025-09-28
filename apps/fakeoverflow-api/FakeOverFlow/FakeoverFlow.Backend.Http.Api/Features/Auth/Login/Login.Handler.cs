using System.Security.Claims;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FakeoverFlow.Backend.Http.Api.Options;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Login;

public static partial class Login 
{
    public class Handler(
        IUserService userService,
        IConfiguration configuration
        ) : Endpoint<Request,
    Results<Ok<Response>, ErrorResponse, ProblemDetails>
    >
    {
        
        public override void Configure()
        {
            Description(x =>
            {
                x.WithName("Login");
            });
            Post("/login");
            AllowAnonymous();
            Group<AuthGroup>();
            Validator<Validator>();
        }

        public override async Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            var loginResponse = await userService.LoginAsync(request: req, cancellationToken: ct);
            if (loginResponse.IsFailure || loginResponse.Value is null)
                return loginResponse.Error!.ToFastEndpointError();

            var tokenId = Ulid.NewUlid().ToString();
            
            var refreshToken = await userService.CreateRefreshTokenForUserAsync(userId: loginResponse.Value.Id, jti: tokenId, cancellationToken: ct);
            if (refreshToken.IsFailure)
                return refreshToken.Error!.ToFastEndpointError();
            
            var jwtOptions = configuration.GetSection("JwtSettings").Get<JwtOptions>();
            if (jwtOptions is null)
                return new ErrorResponse()
                {
                    Message = "JwtSettings not configured",
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            var tokenExpiryOn = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes);
            var token = JwtBearer.CreateToken(loginResponse.Value!.JwtCreationOptions(jwtOptions, tokenExpiryOn, tokenId));
            
            var x = TypedResults.Ok(new Login.Response()
            {
                AccessToken = token,
                RefreshToken = refreshToken.Value!.Id,
                AccessTokenExpires = tokenExpiryOn,
                RefreshTokenExpires = refreshToken.Value.ExpiresOn,
            });
            return x;
        }
    }
}