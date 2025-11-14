using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Microsoft.Agents.AI;
using OpenAI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents;

public class AnalyzerAgents
{
    private readonly OpenAIClient _reasoningClient;
    private readonly OpenAIClient _baseClient;
    
    public AnalyzerAgents(OpenAIClient reasoningClient, OpenAIClient baseClient)
    {
        _reasoningClient = reasoningClient;
        _baseClient = baseClient;
        ClaimExtractor = CreateClaimExtractionAgent();
        MetadataExtractor = CreateClaimMetadataExtractionAgent();
        PlannerAgent = CreatePlannerAgent();
    }

    private AIAgent CreatePlannerAgent()
    {
        return _reasoningClient
            .GetChatClient(PlanGeneratorAgent.ModelName)
            .CreateAIAgent(PlanGeneratorAgent.Options);
    }

    private AIAgent CreateClaimExtractionAgent()
    {
        return _reasoningClient
            .GetChatClient(ClaimExtractorAgent.ModelName)
            .CreateAIAgent(ClaimExtractorAgent.Options);
    }

    private AIAgent CreateClaimMetadataExtractionAgent()
    {
        return _reasoningClient
            .GetChatClient(MetadataExtractorAgent.ModelName)
            .CreateAIAgent(MetadataExtractorAgent.Options);
    }


    public AIAgent ClaimExtractor { get; }

    public AIAgent MetadataExtractor { get; }
    
    public AIAgent PlannerAgent { get; }
    
    public AIAgent WebSummarizerAgent { get; set; }
}