using NSwag.Annotations;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.ListContents;

public partial class ListContents
{
    public class Request
    {
        public string PostId { get; set; } = string.Empty;
    }

    public class Response
    {
        public List<PostContent> Answers { get; set; } = [];
    }

    public class PostContent
    {
        public Guid Id { get; set; }
        
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        public bool IsInternal { get; set; }
        
        public required UserActivity CreatedOn { get; set; }
        
    }
    
    public class UserActivity
    {
        public Guid UserId { get; set; }
        
        public DateTimeOffset ActivityOn { get; set; }
        public required string Username { get; set; }
    }
}