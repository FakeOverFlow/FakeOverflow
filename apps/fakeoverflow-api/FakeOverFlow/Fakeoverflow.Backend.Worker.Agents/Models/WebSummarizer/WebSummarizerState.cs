using System.Text;
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

    public string AsPlanResponseString()
    {
        StringBuilder builder = new();
        builder.AppendLine("Queries Generated: ");
        builder.AppendLine(string.Join($"{Environment.NewLine}  -", QueriesGenerated));
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Evidence Collected: ");
        builder.AppendLine(string.Join($"{Environment.NewLine}  -", EvidenceCollected));

        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Knowledge Gaps: ");
        builder.AppendLine(string.Join($"{Environment.NewLine}  -", KnowledgeGaps));
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Final Verdict: ");
        builder.Append(FinalVerdict);
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Reasoning: ");
        builder.Append(Reasoning);
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Citations: ");
        builder.AppendLine(string.Join($"{Environment.NewLine}  -", Citations));
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Validation Criteria: ");
        builder.AppendLine(string.Join($"{Environment.NewLine}  -", ValidationCriteria));
        
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("Current Time: ");
        builder.Append(CurrentTime);
        
        
        return builder.ToString();
    }
}