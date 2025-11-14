using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;

public class PlanGeneratorExecutor(AIAgent agent) : ReflectingExecutor<PlanGeneratorExecutor>(nameof(PlanGeneratorExecutor)), IMessageHandler<AnalyzerState, AnalyzerState>
{
    public async ValueTask<AnalyzerState> HandleAsync(AnalyzerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please generate a plan for the following post data:

                             PostTitle: {message.PostTitle}
                             Claim: {string.Join(", ", message.ExtractorResponse.ClaimsMade.Select(x => x.ToString()))}
                             Context: {message.ExtractorResponse.Context}
                             Tags: {string.Join(", ", message.PostTags)}
                             Plans: {string.Join(", ", message.Plans)}
                             AdditionalContextForNextPlan: {message.AdditionalContextForNextPlan}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<PlanGeneratorAgent.PlanGeneratorResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }
        
        message.Plans = response?.Plans ?? [];
        return message;
    }
}