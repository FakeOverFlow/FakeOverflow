namespace Fakeoverflow.Backend.Worker.Agents.Models;

public class AnalyzerRequest
{
    public string Title { get; init; } = null!;

    public string Content { get; init; } = null!;

    public List<string> Tags { get; init; } = [];
}