namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Post;

public static partial class Posts
{
    public class Request
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public Guid CreatedBy { get; set; } = Guid.Empty;
    }

    public class Response
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public Guid CreatedBy { get; set; }
    }

    public class InvalidResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}