namespace FakeoverFlow.Backend.Http.Api.Constants;

public static partial class AppConstants
{
    public const string CorsPolicy = "FakeoverFlow.CorsPolicy";
    
    public const string DbVectorTextLanguage = "english";
    
    public static class ConfigMagicKeys
    {
        public static class Jwt
        {
            public const string SigningKey = "Jwt:SigningKey";
            public const string Issuer = "Jwt:Issuer";
            public const string Audience = "Jwt:Audience";
            public const string Expiration = "Jwt:Expiration";
            public const string RefreshExpiration = "Jwt:RefreshExpiration";
        }
    }

}