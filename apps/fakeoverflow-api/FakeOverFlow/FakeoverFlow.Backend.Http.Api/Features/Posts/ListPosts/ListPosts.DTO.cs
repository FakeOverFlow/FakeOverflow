namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ListPosts;

public static partial class ListPosts
{
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
    }
}