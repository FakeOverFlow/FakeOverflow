namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ViewPost;

public static partial class ViewPost
{
    public class Response
    {
        public string PostId { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = [];

        public UserActivity CreatedOn { get; set; } = null!;

        public UserActivity? UpdatedOn { get; set; } = null;
        
        public long Views { get; set; }
        
        public long Votes { get; set; }
    }

    public class UserActivity
    {
        public UserDetails User { get; set; } = null!;
        
        public DateTimeOffset ActivityOn { get; set; }
    }

    public class UserDetails
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string ProfilePicture { get; set; } = string.Empty;
    }
}