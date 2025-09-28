using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Refresh;

internal partial class Refresh
{

    public class Request
    {
        [FromHeader("X-Refresh-Token", true)]
        public string RefreshToken { get; set; } = string.Empty;
        
        [FromHeader("Authorization", true)]
        public string AccessToken { get; set; } = string.Empty;
    }
    
    public class Response
    {
        public string AccessToken { get; set; } = string.Empty;
        
        public string RefreshToken { get; set; } = string.Empty;
        
        public DateTimeOffset AccessTokenExpires { get; set; }
        
        public DateTimeOffset RefreshTokenExpires { get; set; }
    }
}