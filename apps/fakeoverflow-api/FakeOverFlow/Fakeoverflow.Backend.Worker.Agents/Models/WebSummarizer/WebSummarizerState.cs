using Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;

namespace Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;

public class WebSummarizerState
{
    
    
    public string OrginalPost { get; set; } = null!;

    public string VerificationPlan { get; set; } = null!;
    
    public string AdditionalContext { get; set; } = string.Empty;

    public int CurrentIteration { get; set; } = 0;

    public List<string> QueriesGenerated { get; } = [];
    
    public Dictionary<string, string> SearchResults { get; } = new();
    
    
    public List<string> ClaimsExtracted { get; } = [];
    
    public List<EvidenceCollected> EvidenceCollected { get; } = [];

    public List<string> KnowledgeGaps { get; } = [];
    
    public VerificationStatus Status { get; set; }
    
    public List<string> ValidationCriteria { get; } = [];
    
    
    public string FinalVerdict { get; set; } = string.Empty;
    
    public int ConfidenceScore { get; set; } = 0;
    
    public List<string> Citations { get; } = [];
    
    public string Reasoning { get; set; } = string.Empty;
    
    public string CurrentTime = DateTime.UtcNow.ToString("o");
    
    public VerdictCompilerAgent.VerdictCompilerAgentResponse Verdict { get; set; }
    
    // State For Each Agents
    public List<PlanInterperterAgent.PlanInterperterAgentResponse> PlanInterperterAgentResponse { get; set; } = [];
    public List<QueryGeneratorAgent.QueryGeneratorAgentResponse> QueryGeneratorAgentResponse { get; set; } = [];
    
    public List<EvidenceAnalyzerAgent.EvidenceAnalyzerAgentResponse> EvidenceAnalyzerAgentResponse { get; set; } = [];
    public enum VerificationStatus
    {
        InProgress,
        Completed,
        InsufficentEvidence,
    }
}