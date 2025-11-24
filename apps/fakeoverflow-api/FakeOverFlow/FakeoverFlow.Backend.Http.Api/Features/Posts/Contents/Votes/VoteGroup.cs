using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.Votes;

public sealed class VoteGroup : Group
{
    public VoteGroup()
    {
        Configure("/{contentId}/votes", definition =>
        {
            definition.Group<ContentsGroup>();
            definition.Description(b =>
            {
                b.WithGroupName("Post Content");
                b.WithSummary("Post Content of a post related endpoints");
                b.WithDescription("Endpoints for post content related operations");
            });
            definition.EndpointVersion(0);
        });
    }
}