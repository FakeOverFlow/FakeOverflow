namespace FakeoverFlow.Backend.Http.Api.Abstracts.DTO;

public class FactCheckResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Claim { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Tags { get; set; }
    public List<string>? Queries { get; set; }
    public string? Verdict { get; set; }
    public string? Timestamp { get; set; }
}