namespace Fakeoverflow.Backend.Worker.Agents.Models;

public class WebSummarizerRequest
{
    public string SourcePlan { get; init; } = string.Empty;
    
    public string AdditionalContext { get; init; } = string.Empty;
    public string Post { get; set; } = string.Empty;
}