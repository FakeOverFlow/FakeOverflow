using NSwag.Annotations;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.CreateContent;

public partial class CreateContent
{
    public class Request
    {
        public string Content { get; set; }
        
        [OpenApiIgnore]
        public string PostId { get; set; } = string.Empty;
        
        public bool IsInternal { get; set; }
    }

    public class Response
    {
        public required string PostId { get; set; }
        
        public Guid Id { get; set; }
    }
}