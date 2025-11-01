using FakeoverFlow.Backend.Http.Api.Models.Accounts;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class ContentVotes
{
    public Guid UserId { get; set; }

    public UserAccount User { get; set; } = null!;
    
    public Guid ContentId { get; set; }
    
    public PostContent PostContent { get; set; }
    
    public bool UpVote { get; set; }
}