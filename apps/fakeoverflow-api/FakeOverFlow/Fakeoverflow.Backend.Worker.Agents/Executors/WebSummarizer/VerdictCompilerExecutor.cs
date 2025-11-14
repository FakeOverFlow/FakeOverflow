using System.Text.Json;
using Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;
using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;


public class VerdictCompilerExecutor(AIAgent agent) : ReflectingExecutor<VerdictCompilerExecutor>(nameof(VerdictCompilerExecutor)),
    IMessageHandler<WebSummarizerState, WebSummarizerState>
{
    public async ValueTask<WebSummarizerState> HandleAsync(WebSummarizerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var flattenedEvidence = message.EvidenceCollected.Select(e => new
        {
            Claim = e.Claim,
            Support = e.Support.ToString(),
            SupportSummary = e.Reasoning,
            SourceUrls = e.Sources.Select(s => s.Url).Distinct().ToList()
        }).ToList();

        string flattenedEvidenceJson = JsonSerializer.Serialize(flattenedEvidence);
        ChatMessage chatMessage = new(ChatRole.User, [
            new TextContent($"""
                             Please analyze for the following post data:

                             ClaimsExtracted: {message.ClaimsExtracted}
                             EvidenceCollected: {JsonSerializer.Serialize(flattenedEvidenceJson)}
                             ValidationCriteria: {string.Join(", \n", message.ValidationCriteria)}
                             searchIteration: {message.CurrentIteration}
                             OriginalPost: {message.OrginalPost}
                             VerificationPlan: {message.VerificationPlan}
                             AdditionalContext: {message.AdditionalContext}
                             CurrentDateTime: {message.CurrentTime}
                             """)
        ]);
        
        var agentRunResponse = await agent.RunAsync(chatMessage, cancellationToken: cancellationToken);
        if (!agentRunResponse.TryDeserialize<VerdictCompilerAgent.VerdictCompilerAgentResponse>(Constants.JsonOptions, out var response))
        {
            Console.WriteLine("Failed to deserialize response");
        }

        message.FinalVerdict = response?.FinalVerdict ?? false.ToString();
        message.ConfidenceScore = response?.ConfidenceScore ?? 0;
        message.Reasoning = response?.OverallReasoning ?? string.Empty;
        message.Verdict = response!;
        return message;
    }
}