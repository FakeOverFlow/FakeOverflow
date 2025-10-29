using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts;

public class PostGroup : Group
{
    public PostGroup()
    {
        Configure("post", (e) =>
        {
            e.Description(x =>
            {
                x.WithGroupName("Post");
                x.WithSummary("User related endpoints");
                x.WithDescription("Endpoints for me related operations");
            });
            e.EndpointVersion(0);
        });
    }
}