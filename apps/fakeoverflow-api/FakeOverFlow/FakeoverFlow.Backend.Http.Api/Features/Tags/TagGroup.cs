using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Tags;

public sealed class TagGroup : Group
{
    public TagGroup()
    {
        Configure("tags", (e) =>
        {
            e.Description(x =>
            {
                x.WithGroupName("Tags");
                x.WithSummary("Tags related endpoints");
                x.WithDescription("Endpoints for tags related operations");
            });
            e.EndpointVersion(0);
        });
    }
}