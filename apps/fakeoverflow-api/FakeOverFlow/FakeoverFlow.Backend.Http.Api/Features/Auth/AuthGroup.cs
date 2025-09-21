using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth;

public sealed class AuthGroup : Group
{
    public AuthGroup()
    {
        Configure("auth", (e) =>
        {
            e.Description(x =>
            {
                x.WithSummary("Authentication related endpoints");
                x.WithDescription("Endpoints for authentication related operations");
            });
            e.EndpointVersion(0);
        });
    }
}