namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ListPosts;

public static partial class ListPosts
{
    public class Request
    {
        public string? SearchTerm { get; set; } = string.Empty;

        public string? Tag { get; set; } = null;
        
        public int? Page { get; set; } = 1;
        
        public int? PageSize { get; set; } = 30;
    }
    
    public class Response
    {
        public List<PostSummary> Posts { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class PostSummary
    {
        public string PostId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = [];

        public long Views { get; set; }

        public long Votes { get; set; }
        
        public Guid UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;
        
        public DateTimeOffset CreatedOn { get; set; }
    }
}