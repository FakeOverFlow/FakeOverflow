namespace FakeoverFlow.Backend.Http.Api.Options;

public class JwtOptions
{
    public string SigningKey { get; set; } = null!;
    
    public string Issuer { get; set; } = null!;
    
    public string Audience { get; set; } = null!;

    public int ExpiryMinutes { get; set; } = 30;
}