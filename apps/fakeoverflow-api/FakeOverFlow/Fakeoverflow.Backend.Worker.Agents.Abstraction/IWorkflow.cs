using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace Fakeoverflow.Backend.Worker.Agents.Abstraction;

/// <summary>
/// Defines the structure for a workflow interface that processes input data
/// and produces a result, while handling potential exceptions during execution.
/// </summary>
/// <typeparam name="TInput">The type of the input data that the workflow processes.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the workflow.</typeparam>
public interface IWorkflow<in TInput, TResult>
{
    /// <summary>
    /// Executes the workflow with the provided input and returns the result along with any encountered exception.
    /// </summary>
    /// <param name="input">The input data to process within the workflow.</param>
    /// <param name="cancellationToken">A token used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation, containing a tuple with the result of the workflow and any exception encountered during execution.</returns>
    public Task<(TResult?, Exception?)> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    
    
    public Workflow Workflow { get; }
}