using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Workflows;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;

public class PlanExecutor(WebSummarizerWorkflow agent) : ReflectingExecutor<PlanExecutor>(nameof(PlanExecutor)), IMessageHandler<AnalyzerState, AnalyzerState>
{
    public async ValueTask<AnalyzerState> HandleAsync(AnalyzerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var analyserPlan in message.Plans)
        {
            var response = await agent.ExecuteAsync(new WebSummarizerRequest()
            {
                Post = message.PostContent,
                AdditionalContext = analyserPlan.AdditionalContext,
                SourcePlan = analyserPlan.Plan,
            }, cancellationToken);
            
            if(response.Item1 != null)
                message.WebSummaryResult[analyserPlan.Id] = response.Item1;
        }

        return message;
    }
}