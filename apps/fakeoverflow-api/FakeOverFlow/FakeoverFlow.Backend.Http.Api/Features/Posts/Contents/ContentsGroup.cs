using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents;

public sealed class ContentsGroup : Group
{
    public ContentsGroup()
    {
        Configure("/{postId}/contents", x =>
        {
            x.Group<PostGroup>();
            x.Description(b =>
            {
                b.WithGroupName("Post Content");
                b.WithSummary("Post Content of a post related endpoints");
                b.WithDescription("Endpoints for post content related operations");
            });
            x.EndpointVersion(0);
        });
    }
}