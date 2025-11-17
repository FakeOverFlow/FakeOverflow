using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;

namespace Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;

public class AnalyzerState
{
    public required string PostTitle { get; init; }
    
    public required string PostContent { get; init; }

    public required List<string> PostTags { get; init; } = [];

    public ClaimExtractorAgent.ClaimExtractorResponse ExtractorResponse { get; set; } = null!;
    
    public MetadataExtractorAgent.MetadataExtractorResponse MetadataResponse { get; set; } = null!;
    
    public List<AnalyserPlan> Plans { get; set; } = [];
    
    public string AdditionalContextForNextPlan { get; set; } = string.Empty;

    public Dictionary<Guid, WebSummarizerState> WebSummaryResult = new();
    
    public FinalVerdictAgent.FinalVerdictAgentResponse FinalVerdict { get; set; }
}

