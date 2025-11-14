using System.Text;

namespace Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;

public class AnalyserPlan
{
    
    public Guid Id { get; private set; } = Guid.NewGuid();
    public AnalyserStep Step { get; set; }

    public string Plan { get; set; } = null!;
    
    public string AdditionalContext { get; set; } = string.Empty;
    
    
    public enum AnalyserStep
    {
        WebSearch,
        SourceCrossCheck,
        AuthorityVerification,
        ConsistencyCheck,
        DomainExpertCheck
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append("Step type: ");
        builder.Append(Step);
        
        builder.Append(Environment.NewLine);
        
        builder.Append("Plan: ");
        builder.Append(Plan);
        
        builder.Append(Environment.NewLine);
        
        builder.Append("Additional Context: ");
        builder.Append(AdditionalContext);
        return builder.ToString();
    }
}