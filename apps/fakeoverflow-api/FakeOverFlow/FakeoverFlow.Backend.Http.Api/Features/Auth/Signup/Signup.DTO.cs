namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;

public static partial class Signup
{
    public class Request
    {
        public string Email { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
    }

    public class Response
    {
        public Guid UserId { get; set; }
        
        public string Email { get; set; } = string.Empty;
    }
}