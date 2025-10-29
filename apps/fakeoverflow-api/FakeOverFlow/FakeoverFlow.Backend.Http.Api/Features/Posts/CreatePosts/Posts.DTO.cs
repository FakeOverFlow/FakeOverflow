namespace FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts;

public static partial class Posts
{
    public class Request
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<string> Tags { get; set; } = [];
    }

    public class Response
    {
        public string Id { get; set; } = null!;
    }
}