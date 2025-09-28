namespace FakeoverFlow.Backend.Http.Api.Features.Me.Get;

public static partial class GetMe
{
    public class Response
    {
        public string Id { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;
        
        public string? ProfilePicture { get; set; } = string.Empty;
        
        public MeResponseSettings Settings { get; set; } = new();
    }
    
    public class MeResponseSettings
    {
    }
}