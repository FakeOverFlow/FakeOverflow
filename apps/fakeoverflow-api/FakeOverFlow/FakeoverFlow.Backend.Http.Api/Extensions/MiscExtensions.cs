using System.Security.Claims;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using FakeoverFlow.Backend.Http.Api.Options;
using FastEndpoints.Security;

namespace FakeoverFlow.Backend.Http.Api.Extensions;

public static class MiscExtensions
{
    public static Action<JwtCreationOptions> JwtCreationOptions(
        this UserAccount userAccount,
            JwtOptions jwtOptions,
            DateTime tokenExpiryOn,
            string jti
        )
    {
        return (x) =>
        {
            x.SigningKey = jwtOptions.SigningKey;
            x.ExpireAt = tokenExpiryOn;
            x.User.Roles.Add(nameof(userAccount.Role));
            x.Audience = jwtOptions.Audience;
            x.Issuer = jwtOptions.Issuer;
            x.User.Claims.Add(new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()));
            ;
            x.User.Claims.Add(new Claim(ClaimTypes.Name, userAccount.Username));
            ;
            x.User.Claims.Add(new Claim(ClaimTypes.Email, userAccount.Email));
            ;
            x.User.Claims.Add(new Claim(ClaimTypes.GivenName, userAccount.FirstName));
            ;
            x.User.Claims.Add(new Claim(ClaimTypes.Surname, userAccount.LastName));
            ;
            x.User.Claims.Add(new Claim("jti", jti));
        };
    }
}