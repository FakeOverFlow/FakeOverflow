using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Me;

public sealed class MeGroup : Group
{
    public MeGroup()
    {
        Configure("me", (e) =>
        {
            e.Description(x =>
            {
                x.WithGroupName("Me");
                x.WithSummary("User related endpoints");
                x.WithDescription("Endpoints for me related operations");
            });
            e.EndpointVersion(0);
        });
    }
}