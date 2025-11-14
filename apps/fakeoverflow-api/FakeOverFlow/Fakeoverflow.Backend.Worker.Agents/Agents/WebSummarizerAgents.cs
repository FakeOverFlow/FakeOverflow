using Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;
using Microsoft.Agents.AI;
using OpenAI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents;

public class WebSummarizerAgents
{
    private readonly OpenAIClient _reasoningClient;
    private readonly OpenAIClient _baseClient;

    public AIAgent PlanInterperter { get; }
    public AIAgent QueryGenerator { get; }
    public AIAgent EvidenceAnalyzer { get; }
    public AIAgent VerdictCompiler { get; }

    public WebSummarizerAgents(OpenAIClient reasoningClient, OpenAIClient baseClient)
    {
        _reasoningClient = reasoningClient;
        _baseClient = baseClient;
        
        PlanInterperter = _reasoningClient.GetChatClient(PlanInterperterAgent.ModelName)
            .CreateAIAgent(PlanInterperterAgent.Options);
        QueryGenerator = _reasoningClient.GetChatClient(QueryGeneratorAgent.ModelName)
            .CreateAIAgent(QueryGeneratorAgent.Options);
        EvidenceAnalyzer = _reasoningClient.GetChatClient(EvidenceAnalyzerAgent.ModelName)
            .CreateAIAgent(EvidenceAnalyzerAgent.Options);
        VerdictCompiler = _baseClient.GetChatClient(VerdictCompilerAgent.ModelName)
            .CreateAIAgent(VerdictCompilerAgent.Options);
    }
}