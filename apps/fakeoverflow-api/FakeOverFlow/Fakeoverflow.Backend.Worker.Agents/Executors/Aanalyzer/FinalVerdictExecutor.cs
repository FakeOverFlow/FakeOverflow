using System.Text;
using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;

public class FinalVerdictExecutor(AIAgent agent) : ReflectingExecutor<FinalVerdictExecutor>(nameof(FinalVerdictExecutor)), IMessageHandler<AnalyzerState, AnalyzerState>
{
    public async ValueTask<AnalyzerState> HandleAsync(AnalyzerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please analyze and create verdict for the following post data:

                             Original Post:
                                PostTitle: {message.PostTitle}
                                PostContent: {message.PostContent}
                                Tags: {string.Join(", ", message.PostTags)}
                                
                             ExtractedClaims: {string.Join(", ", message.ExtractorResponse.ClaimsMade)}
                             MetadataExtractedFromPost: {message.MetadataResponse.Metadata.ToString()}
                             VerificationPlans: {
                                 string.Join(","+Environment.NewLine, message.Plans.Select(x => $"PlanId: {x.Id}{Environment.NewLine}Plan: {x.Plan}{Environment.NewLine}AdditionalContext: {x.AdditionalContext}"))
                             }
                             WebSearchResults: {
                                 string.Join(", "+Environment.NewLine, message.WebSummaryResult.Select((pair => $"PlanId: {pair.Key}{Environment.NewLine}{pair.Value.AsPlanResponseString()}")).ToList())
                             }
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<FinalVerdictAgent.FinalVerdictAgentResponse>(Constants.JsonOptions,
                out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }

        message.FinalVerdict = response!;
        return message;
    }
}