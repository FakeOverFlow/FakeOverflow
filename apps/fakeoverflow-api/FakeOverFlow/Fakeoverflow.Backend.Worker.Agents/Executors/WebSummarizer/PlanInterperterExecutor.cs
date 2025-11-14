using Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;
using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;

public class PlanInterperterExecutor(AIAgent agent) : ReflectingExecutor<PlanInterperterExecutor>(nameof(PlanInterperterExecutor)),
    IMessageHandler<WebSummarizerState, WebSummarizerState>
{
    public async ValueTask<WebSummarizerState> HandleAsync(WebSummarizerState message, IWorkflowContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please plan for the following post data:
                             
                             OriginalPost: {message.OrginalPost}
                             
                             VerificationPlan: {message.VerificationPlan}
                             
                             AdditionalContext: {message.AdditionalContext}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<PlanInterperterAgent.PlanInterperterAgentResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }
        
        message.ClaimsExtracted.AddRange(response?.ClaimsExtracted ?? []);
        message.ValidationCriteria.AddRange(response?.ValidationCriteria ?? []);
        if(response is not null)
            message.PlanInterperterAgentResponse.Add(response);
        
        return message;
    }
}