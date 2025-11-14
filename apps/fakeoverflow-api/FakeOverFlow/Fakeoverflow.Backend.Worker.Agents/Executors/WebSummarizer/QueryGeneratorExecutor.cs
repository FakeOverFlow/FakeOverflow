using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;

public class QueryGeneratorExecutor(AIAgent agent) : ReflectingExecutor<QueryGeneratorExecutor>(nameof(QueryGeneratorExecutor)),
    IMessageHandler<WebSummarizerState, WebSummarizerState>
{
    public async ValueTask<WebSummarizerState> HandleAsync(WebSummarizerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please plan for the following post data:

                             ClaimsExtracted: {message.PlanInterperterAgentResponse.LastOrDefault()?.ClaimsExtracted ?? []}
                             KnowledgeGaps: {string.Join(",\n", message.PlanInterperterAgentResponse.LastOrDefault()?.KnowledgeGaps ?? [])}
                             QueriesGenerated: {string.Join(", \n", message.QueriesGenerated)}
                             SearchResults: Truncated due to size
                             CurrentTime: {message.CurrentTime}
                             SearchIteration: {message.CurrentIteration}
                             WhyMoreQueries: {message.EvidenceAnalyzerAgentResponse.LastOrDefault()?.Reasoning}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<Agents.Summarizer.QueryGeneratorAgent.QueryGeneratorAgentResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }
        
        message.QueriesGenerated.AddRange(response?.Queries ?? []);
        if(response is not null)
            message.QueryGeneratorAgentResponse.Add(response);
        
        message.CurrentIteration++;
        return message;
    }
}