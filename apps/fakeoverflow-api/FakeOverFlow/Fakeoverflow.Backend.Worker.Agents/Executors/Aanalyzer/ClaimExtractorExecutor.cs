using Fakeoverflow.Backend.Worker.Agents.Agents;
using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;

public class ClaimExtractorExecutor(AIAgent agent) : ReflectingExecutor<ClaimExtractorExecutor>(nameof(ClaimExtractorExecutor)), IMessageHandler<AnalyzerState, AnalyzerState>
{

    public async ValueTask<AnalyzerState> HandleAsync(AnalyzerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                            Please analyze the following post data and extract the claim:
                            
                            PostTitle: {message.PostTitle}
                            PostContent: {message.PostContent}
                            Tags: {string.Join(", ", message.PostTags)}
                            """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        var response = agentRunResponse.Deserialize<ClaimExtractorAgent.ClaimExtractorResponse>(Constants.JsonOptions);
        // if (!agentRunResponse.TryDeserialize<ClaimExtractorAgent.ClaimExtractorResponse>(Constants.JsonOptions, out var response))
        // {
        //     Console.WriteLine("Failed to deserialize response");
        // }
        
        message.ExtractorResponse = response!;
        return message;
    }
}