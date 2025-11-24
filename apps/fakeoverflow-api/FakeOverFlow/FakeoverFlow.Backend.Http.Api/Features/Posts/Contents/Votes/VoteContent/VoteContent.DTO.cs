using NSwag.Annotations;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.Votes.VoteContent;

public partial class VoteContent
{
    public class Request
    {
        [OpenApiIgnore]
        public string PostId { get; set; }
        
        [OpenApiIgnore]
        public Guid ContentId { get; set; }
        
        public bool IsUpvote { get; set; }
    }

    public class Response
    {
        public Guid UserId { get; set; }
        
        public Guid ContentId { get; set; }
    }
}