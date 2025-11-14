using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fakeoverflow.Backend.Worker.Agents;

public class Constants
{
    public static class Models
    {
        public const string ReasoningModel = "microsoft/phi-4-reasoning-plus";
        public const string BaseModel = "qwen/qwen3-4b-2507";

        public const int BaseModelContextWindow = 262144;
    }

    public static class Config
    {
        public static class WebSummarizer
        {
            public const int MaxIteration = 2;
        }
    }
    
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        TypeInfoResolver = System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver.Combine(
            new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
        ),
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };
}