using NSwag.Annotations;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.Votes.DeleteVote;

public partial class DeleteVote
{
    public class Request
    {
        [OpenApiIgnore]
        public Guid ContentId { get; set; }
        
        [OpenApiIgnore]
        public string PostId { get; set; }
    }
}