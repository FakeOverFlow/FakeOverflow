using System.Text;

namespace Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;

public class EvidenceCollected
{
    public string Claim { get; set; } = null!;
    
    public EvidenceSupport Support { get; set; }
    
    public string Reasoning { get; set; }
    
    public List<EvidenceSource> Sources { get; set; }
    public enum EvidenceSupport
    {
        Supported,
        Refuted,
        Inconclusive,
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.AppendLine($"Claim: {Claim}");
        builder.AppendLine($"Support: {Support}");
        builder.AppendLine($"Reasoning: {Reasoning}");
        builder.AppendLine($"Sources: {string.Join(", ", Sources)}");
        
        return builder.ToString();
    }

    public class EvidenceSource
    {
        public string Url { get; set; }
        
        public string Title { get; set; }
        
        public string Snippet { get; set; }
        
        public string PublishDate { get; set; }
        
        public EvidenceSourceCredibility Credibility { get; set; }
        public enum EvidenceSourceCredibility
        {
            High,
            Medium,
            Low,
        }
        
        public override string ToString()
        {
            return $"Url: {Url}, Title: {Title}, Snippet: {Snippet}, PublishDate: {PublishDate}, Credibility: {Credibility}";
        }
    }
}