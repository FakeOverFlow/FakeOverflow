using Fakeoverflow.Backend.Worker.Agents.Abstraction;
using Fakeoverflow.Backend.Worker.Agents.Executors.Aanalyzer;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace Fakeoverflow.Backend.Worker.Agents.Workflows;

public class AnalyzerWorkflow : IWorkflow<AnalyzerRequest, AnalyzerState>
{
    private readonly Agents.AnalyzerAgents _analyzerAgents;
    private readonly Workflow _workflow;
    private readonly WebSummarizerWorkflow _webSummarizerWorkflow;
    
    public AnalyzerWorkflow(Agents.AnalyzerAgents analyzerAgents, WebSummarizerWorkflow webSummarizerWorkflow)
    {
        _analyzerAgents = analyzerAgents;
        _webSummarizerWorkflow = webSummarizerWorkflow;
        _workflow = CreateWorkflow();
    }

    private Workflow CreateWorkflow()
    {
        var claimExtractorExecutor = new ClaimExtractorExecutor(_analyzerAgents.ClaimExtractor);
        var metadataExtractorExecutor = new MetadataExtractorExecutor(_analyzerAgents.MetadataExtractor);
        var planGeneratorExecutor = new PlanGeneratorExecutor(_analyzerAgents.PlannerAgent);
        var planExecutor = new PlanExecutor(_webSummarizerWorkflow);
        var workflowBuilder = new WorkflowBuilder(claimExtractorExecutor);
        workflowBuilder.AddEdge(
            source: claimExtractorExecutor,
            target: metadataExtractorExecutor
        );

        workflowBuilder.AddEdge(
            source: metadataExtractorExecutor,
            target: planGeneratorExecutor
        );

        workflowBuilder.AddEdge(
            source: planGeneratorExecutor,
            target: planExecutor
        );

        
        workflowBuilder.WithOutputFrom(planExecutor);
        return workflowBuilder.Build();
    }
    
    public async Task<(AnalyzerState?, Exception?)> ExecuteAsync(AnalyzerRequest input, CancellationToken cancellationToken = default)
    {
        var inputState = new AnalyzerState()
        {
            PostContent = input.Content,
            PostTags = input.Tags,
            PostTitle = input.Title,
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

    public AIAgent AsAgent()
    {
        return _workflow.AsAgent();
    }

    public Workflow Workflow => _workflow;
}