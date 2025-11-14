// See https://aka.ms/new-console-template for more information

using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fakeoverflow.Backend.Worker.Agents;
using Fakeoverflow.Backend.Worker.Agents.Agents;
using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Workflows;
using Microsoft.Agents.AI.Workflows;
using OpenAI;

public static class Program
{
    public static async Task Main()
    {
        OpenAIClient reasoningClient = new(new ApiKeyCredential("123"), new OpenAIClientOptions()
        {
            Endpoint = new Uri("http://127.0.0.1:1234/v1/"),
            NetworkTimeout = TimeSpan.FromMinutes(10)
        });

        OpenAIClient baseClient = new(new ApiKeyCredential("123"), new OpenAIClientOptions()
        {
            Endpoint = new Uri("http://127.0.0.1:1234/v1/"),
            NetworkTimeout = TimeSpan.FromMinutes(10)
        });

        AnalyzerAgents analyzerAgents = new(reasoningClient, baseClient);
        WebSummarizerAgents webSummarizerAgents = new(reasoningClient, baseClient);
        WebSummarizerWorkflow webSummarizerWorkflow = new WebSummarizerWorkflow(webSummarizerAgents);
        AnalyzerWorkflow workflow = new(analyzerAgents, webSummarizerWorkflow);

        // var y = await webSummarizerWorkflow.ExecuteAsync(new WebSummarizerRequest()
        // {
        //     Post = "Apple has launched iphone 17 air, the thinnest iphone ever released. is this true guys?",
        //     AdditionalContext = "The post claims the iPhone 17 Air is the thinnest iPhone ever released, but there is no mention of an official launch or published specifications. The claim appears to stem from a speculative or unverified source. This plan aims to verify whether the iPhone 17 Air exists as an official Apple product and whether its thickness has been confirmed.",
        //     SourcePlan = "Claim: Apple has launched the iPhone 17 Air, the thinnest iPhone ever released.\\n  What Still Needs to Be Learned: \\\\- Whether the iPhone 17 Air has been officially announced or released by Apple.\\\\- If Apple has ever released an iPhone model thinner than any previous model (e.g., iPhone 14 Pro, iPhone 15 Pro, or iPhone 13 Pro Max).\\\\- The exact thickness specifications of the iPhone 17 Air (if any) and how it compares to prior iPhone models.\\\\- Whether the claim is based on a leaked report, speculation, or a verified product launch.\\n  How to Search: \\\\- Search for official Apple press releases, product announcements, or investor presentations mentioning the 'iPhone 17 Air'.\\\\- Look for verified news reports from reputable tech outlets (e.g., The Verge, CNN Tech, Bloomberg, Reuters) that confirm or deny the launch of the iPhone 17 Air.\\\\- Search for official specifications (e.g., thickness, weight, dimensions) of the iPhone 17 Air, and compare them to the thinnest previous iPhone models.\\\\- Use search operators to distinguish between confirmed launches and rumors or leaks (e.g., 'iPhone 17 Air official announcement', 'iPhone 17 Air specifications', 'Apple released iPhone 17 Air 2024').\\\\- Filter results to focus on credible, recent (within the last 12 months), and verified sources.\\n  Example Queries (These are examples. The next agent can adapt them):\\\\- 'Apple iPhone 17 Air official announcement 2024' \\\\- 'iPhone 17 Air thickness specifications Apple' \\\\- 'thinnest iPhone ever released official record' \\\\- 'Apple product launch event 2024 iPhone 17 Air' \\\\- 'iPhone 15 Pro vs iPhone 14 Pro thickness comparison' \\\\- 'iPhone 13 Pro Max thickness official measurement'\\n  Reliable Sources to Prioritize: \\\\- Official Apple website (apple.com) and support pages \\\\- Apple's Investor Relations or Events page (ir.apple.com) \\\\- Reputable technology news sources: The Verge, Bloomberg, Reuters, CNET, TechCrunch, Wired \\\\- Government or regulatory filings (e.g., FCC databases) if relevant to product specs \\\\- Academic or industry benchmarks on smartphone design (e.g., from IEEE or research papers)\\n  Validation Criteria for Summary: \\\\- The claim is supported only if: \\\\- There is an official Apple announcement confirming the iPhone 17 Air launch. \\\\- The thickness of the iPhone 17 Air is publicly documented and explicitly stated as the thinnest ever. \\\\- The data is consistent across at least two independent, credible sources (e.g., Apple + a major tech outlet). \\\\- If no official launch exists or if the thickness is not verified, the claim is not supported. \\\\- If the iPhone 17 Air is a rumor or leak, and no official confirmation exists, the claim is unsupported.\\n  Additional Context: \\\\- This plan addresses the core factual claim that Apple has launched the iPhone 17 Air and that it is the thinnest iPhone ever released. \\\\- Prior to this, no plans have addressed the existence of the iPhone 17 Air or its specifications. \\\\- The post raises skepticism about the claim, so verifying its authenticity and factual basis is essential. \\\\- This search will determine whether the claim is based on real product data or speculative reporting."
        // });
        
        var x = await workflow.ExecuteAsync(new AnalyzerRequest()
        {
            Title = "Apple Iphone Air, The thinnest iphone ever release",
            Content = "Apple has launched iphone air, the thinnest iphone ever released. I see it is launched with other Iphone 17 series, I also heard that Iphone 17 has a VR Camera is this true guys?",
            Tags = ["iphone", "apple", "tech"]
        });

        var y2 = await workflow.ExecuteAsync(new AnalyzerRequest()
        {
            Tags = ["us", "donald trump", "politics"],
            Content = "Kim Jong Un has been elected president of the United States.",
            Title = "Kim Jong Un has been elected president of the United States."
        });
        
        // Console.WriteLine(JsonSerializer.Serialize(x, Constants.JsonOptions));
        Console.WriteLine(JsonSerializer.Serialize(y2, Constants.JsonOptions));
        Console.ReadLine();

    }
}