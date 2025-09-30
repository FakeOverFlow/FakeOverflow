namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Available;

public static partial class Available
{

    public enum AvailabilityType
    {
        Username,
        Email
    }
    
    public class Request
    {
        public AvailabilityType Type { get; set; } 
        
        public string Value { get; set; } = string.Empty;
    }
    
    
}