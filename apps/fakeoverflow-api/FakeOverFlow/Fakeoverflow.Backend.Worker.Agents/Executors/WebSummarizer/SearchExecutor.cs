using Fakeoverflow.Backend.Worker.Agents.Models.WebSummarizer;
using Fakeoverflow.Backend.Worker.Agents.Tools;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Fakeoverflow.Backend.Worker.Agents.Executors.WebSummarizer;

public class SearchExecutor() : ReflectingExecutor<SearchExecutor>(nameof(SearchExecutor)), IMessageHandler<WebSummarizerState, WebSummarizerState>
{
    public async ValueTask<WebSummarizerState> HandleAsync(WebSummarizerState message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var queryGeneratorAgentResponse = message.QueryGeneratorAgentResponse.LastOrDefault();
        if(queryGeneratorAgentResponse == null)
            return message;
        
        foreach (var query in queryGeneratorAgentResponse.Queries)
        {
            Console.WriteLine($"Searching {query}");
            var searchSerper = await SerperWebSearch.SearchSerper(query);

            message.SearchResults[query] = searchSerper;
        }

        return message;
    }
}