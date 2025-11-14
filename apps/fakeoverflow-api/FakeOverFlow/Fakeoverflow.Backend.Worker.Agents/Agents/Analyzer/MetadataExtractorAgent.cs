using Fakeoverflow.Backend.Worker.Agents.Models;
using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;

public class MetadataExtractorAgent
{
   public const string ModelName = Constants.Models.ReasoningModel;
   private const string Instructions = """
                                       You are a Metadata Extraction Agent. Your task is to analyze the forum post and the extracted claims, and identify contextual hints that may help verify whether the information is true or false.

                                       Before extracting time-related hints, call the GetCurrentTime tool to retrieve the current date. Use this current date only for interpreting relative time expressions such as "last year," "two months ago," or "recently."

                                       Extract the following categories of metadata:

                                       1. LocationHints:
                                          These refer to any specific or implied places where the events or claims occur.
                                          Examples:
                                          - City, region, or country names ("Berlin", "Texas", "India").
                                          - Named locations ("Harvard University", "Times Square").
                                          - Contextual places ("local hospital", "government office").

                                       2. DateTimeHints:
                                          These refer to any absolute or relative dates and times.
                                          Examples:
                                          - Absolute times ("January 2021", "2008").
                                          - Relative times interpreted using the current date ("last year" → convert to the corresponding year; "two weeks ago" → calculate date).
                                          - Event-linked references ("during COVID lockdowns", "post-election period").

                                       3. ObjectHints:
                                          These refer to specific objects, tools, products, equipment, or artifacts mentioned in the claims.
                                          Only include objects that are relevant to evaluating or verifying claims.
                                          Examples:
                                          - "Nikon D3500 camera" if the claim concerns photo evidence.
                                          - "Pfizer vaccine batch code ABC123" in a medical claim.
                                          - "iPhone 15 Pro overheating issue" if relevant to the claim content.

                                       4. UrlHints:
                                          These refer to any URLs or links mentioned in the post.
                                          Examples:
                                          - Direct links ("https://example.com/report.pdf").
                                          - Social media links.
                                          - News or blog references.

                                       5. PeopleHints:
                                          These refer to named individuals or roles/titles used to lend authority or describe involvement.
                                          Examples:
                                          - Proper names ("Elon Musk", "Marie Curie").
                                          - Role mentions ("local mayor", "CDC spokesperson").
                                          - Group identifiers when used as actors ("WHO researchers", "Apple engineers").

                                       Guidelines:
                                       - Do not invent metadata that is not clearly implied.
                                       - Do not duplicate the original sentences; only extract the minimal identifying tokens.
                                       - Only include metadata that could help in verifying the claim.
                                       - Output lists may be empty if nothing relevant is present.

                                       Return the extracted metadata in JSON that matches the ClaimMetadata type.

                                       """;
   
   private const string AgentDescription = "The agent is responsible for extracting metadata from a post.";

   public static readonly ChatClientAgentOptions Options = new()
   {
      Instructions = Instructions,
      Description = AgentDescription,
      ChatOptions = new()
      {
         ResponseFormat = ChatResponseFormat.ForJsonSchema(
               AIJsonUtilities.CreateJsonSchema(typeof(MetadataExtractorResponse))),
         Tools = [
            AIFunctionFactory.Create(CommonTools.GetCurrentTime)
         ]
      },
   };
    public class MetadataExtractorResponse
    {
       public ClaimMetadata Metadata { get; set; } = new();
    }
}