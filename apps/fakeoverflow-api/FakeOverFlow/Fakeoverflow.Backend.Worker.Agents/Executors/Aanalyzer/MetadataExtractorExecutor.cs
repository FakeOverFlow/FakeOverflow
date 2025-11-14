using Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;

public class MetadataExtractorExecutor(AIAgent agent) : ReflectingExecutor<MetadataExtractorExecutor>(nameof(MetadataExtractorExecutor)), IMessageHandler<AnalyzerState, AnalyzerState>
{
    public async ValueTask<AnalyzerState> HandleAsync(AnalyzerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please extract the metadata from the following post data:

                             PostTitle: {message.PostTitle}
                             PostContent: {message.PostContent}
                             Tags: {string.Join(", ", message.PostTags)}
                             Context: {message.ExtractorResponse.Context}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<MetadataExtractorAgent.MetadataExtractorResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }
        
        message.MetadataResponse = response!;
        return message;
    }
}