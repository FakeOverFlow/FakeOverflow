namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Login;

internal partial class Login
{
    public enum AuthenticationType
    {
        Credentials,
        Google,
    }
    
    public class Request
    {
        public AuthenticationType Type { get; set; } 
        
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }

    public class Response
    {
        public string AccessToken { get; set; } = string.Empty;
        
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class InvalidResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}