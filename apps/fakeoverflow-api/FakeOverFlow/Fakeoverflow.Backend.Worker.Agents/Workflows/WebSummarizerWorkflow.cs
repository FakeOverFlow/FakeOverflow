using Fakeoverflow.Backend.Worker.Agents.Abstraction;
using Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Microsoft.Agents.AI.Workflows;

namespace Fakeoverflow.Backend.Worker.Agents.Workflows;

public class WebSummarizerWorkflow : IWorkflow<WebSummarizerRequest, WebSummarizerState>
{
    
    private readonly Agents.WebSummarizerAgents _agents;
    private readonly Workflow _workflow;
    
    public WebSummarizerWorkflow(Agents.WebSummarizerAgents agents)
    {
        _agents = agents;
        _workflow = CreateWorkflow();
    }

    private Workflow CreateWorkflow()
    {
        var evidenceAnalyzerExecutor = new EvidenceAnalyzerExecutor(_agents.EvidenceAnalyzer);
        var planInterperterExecutor = new PlanInterperterExecutor(_agents.PlanInterperter);
        var queryGenerationExecutor = new QueryGeneratorExecutor(_agents.QueryGenerator);
        var verdictCompilerExecutor = new VerdictCompilerExecutor(_agents.VerdictCompiler);
        var searchExecutor = new SearchExecutor();

        var workflowBuilder = new WorkflowBuilder(planInterperterExecutor);
        workflowBuilder.AddEdge(planInterperterExecutor, queryGenerationExecutor);
        workflowBuilder = workflowBuilder.AddEdge(queryGenerationExecutor, searchExecutor);
        workflowBuilder = workflowBuilder.AddEdge(searchExecutor, evidenceAnalyzerExecutor);
        workflowBuilder = workflowBuilder.AddEdge<WebSummarizerState>(evidenceAnalyzerExecutor, verdictCompilerExecutor, (arg) => !ShouldRetry(arg));
        workflowBuilder = workflowBuilder.AddEdge<WebSummarizerState>(evidenceAnalyzerExecutor, queryGenerationExecutor, ShouldRetry);
        
        workflowBuilder = workflowBuilder.WithOutputFrom(verdictCompilerExecutor);
        return workflowBuilder.Build();
    }

    private bool ShouldRetry(WebSummarizerState? arg)  {
        if (arg is null)
            return true;
            
        if(arg.CurrentIteration >= Constants.Config.WebSummarizer.MaxIteration)
            return false;
            
        return arg.EvidenceAnalyzerAgentResponse.LastOrDefault()?.NeedsMoreSearches ?? false;
    }
    
    public async Task<(WebSummarizerState?, Exception?)> ExecuteAsync(WebSummarizerRequest input, CancellationToken cancellationToken = default)
    {
        var inputState = new WebSummarizerState()
        {
            OrginalPost = input.Post,
            AdditionalContext = input.AdditionalContext,
            VerificationPlan = input.SourcePlan
        };
        var run = await InProcessExecution.RunAsync(workflow: _workflow, inputState, cancellationToken: cancellationToken);
        
        foreach (WorkflowEvent evt in run.NewEvents)
        {
            switch (evt)
            {
                case ExecutorCompletedEvent executorComplete:
                    Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
                    break;
                case WorkflowOutputEvent workflowOutput:
                    Console.WriteLine($"Workflow '{workflowOutput.SourceId}' outputs: {workflowOutput.Data}");
                    break;
                case ExecutorFailedEvent failedEvent:
                    Console.WriteLine($"Executor '{failedEvent.ExecutorId}' failed: {failedEvent.Data}");
                    break;
                default:
                    Console.WriteLine($"Unknown event: {evt.GetType().Name} - {evt.Data}");
                    break;
            }
        }

        return (inputState!, null);
    }

    public Workflow Workflow => _workflow;
}