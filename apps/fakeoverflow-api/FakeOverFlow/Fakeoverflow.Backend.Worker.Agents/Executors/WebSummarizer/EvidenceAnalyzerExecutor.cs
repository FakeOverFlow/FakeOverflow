using System.Text.Json;
using Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;
using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;

public class EvidenceAnalyzerExecutor(AIAgent agent) : ReflectingExecutor<EvidenceAnalyzerExecutor>(nameof(EvidenceAnalyzerExecutor)),
    IMessageHandler<WebSummarizerState, WebSummarizerState>
{
    public async ValueTask<WebSummarizerState> HandleAsync(WebSummarizerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please analyze for the following post data:

                             SearchResults: {ConvertAsString(message.SearchResults)}
                             ClaimsExtracted: {string.Join(", \n", message.ClaimsExtracted)}
                             KnowledgeGaps: {string.Join(", \n", message.KnowledgeGaps)}
                             EvidenceCollected: {JsonSerializer.Serialize(message.EvidenceCollected)}
                             ValidationCriteria: {string.Join(", \n", message.ValidationCriteria)}
                             CurrentIteration: {message.CurrentIteration}
                             MaxIteration: {Constants.Config.WebSummarizer.MaxIteration}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<EvidenceAnalyzerAgent.EvidenceAnalyzerAgentResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }
        
        message.EvidenceCollected.AddRange(response?.EvidenceCollected ?? []);
        if(response is not null)
            message.EvidenceAnalyzerAgentResponse.Add(response);
        
        return message;
    }
    
    private string ConvertAsString(Dictionary<string, string> dictionary) => string.Join(Environment.NewLine, dictionary.Select(x => $"{x.Key}: {x.Value}"));
}