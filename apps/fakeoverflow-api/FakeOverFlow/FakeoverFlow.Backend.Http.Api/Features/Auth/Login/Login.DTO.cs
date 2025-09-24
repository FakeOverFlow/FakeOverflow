namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Login;

public static partial class Login
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
        
        public string? UserName => Data["UserName"];
        public string? Password => Data["Password"];
        public string? Code => Data["Code"];
    }

    public class Response
    {
        public string AccessToken { get; set; } = string.Empty;
        
        public string RefreshToken { get; set; } = string.Empty;
        
        public DateTimeOffset AccessTokenExpires { get; set; }
        
        public DateTimeOffset RefreshTokenExpires { get; set; }
    }

    public class InvalidResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}