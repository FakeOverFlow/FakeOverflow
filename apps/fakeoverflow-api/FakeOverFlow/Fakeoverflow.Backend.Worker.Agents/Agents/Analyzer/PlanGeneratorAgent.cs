using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;

public class PlanGeneratorAgent
{
    public const string ModelName = Constants.Models.BaseModel;

    private const string Instructions = """
                                        You are the Plan Generator Agent.
                                        
                                        You will be given a USER INPUT block containing:
                                        - PostTitle
                                        - Claim (a single claim to evaluate)
                                        - Context
                                        - Tags
                                        - Plans (list of previous research plans, if any exist)
                                        - AdditionalContextForNextPlan (optional clarifications or remaining uncertainties)
                                        
                                        Your goal is to produce exactly ONE new Web Research Plan for the claim.
                                        
                                        Your plan must be actionable and designed for the next agent who will execute searches and summarize results.
                                        
                                        Before generating the plan, perform these checks:
                                        
                                        1. **Check Existing Plans**
                                           - If previous plans already address part of the claim, do not repeat the same search steps.
                                           - If a previous plan gathered certain facts already, your new plan should:
                                             - Narrow the focus,
                                             - Validate contradictions,
                                             - Or expand into missing aspects.
                                           - Reference prior plans by index when building or refining:
                                               Example: "This plan builds on Plan[0] by focusing on verifying the numeric measurement mentioned in the claim."
                                        
                                        2. **Check AdditionalContextForNextPlan**
                                           - If provided, this indicates what information was still unclear during the last summary step.
                                           - Your new plan should focus primarily on obtaining or clarifying that missing information.
                                           - Do not re-request information that was already retrieved.
                                        
                                        Now, generate the plan in the following structure:
                                        
                                        Step: "WebSearch"
                                        Plan:
                                          Claim:
                                            Restate the claim in your own words.
                                        
                                          What Still Needs to Be Learned:
                                            Based on:
                                              - The claim
                                              - Existing Plans
                                              - AdditionalContextForNextPlan
                                            Identify the specific missing pieces of information required to confirm, refute, or contextualize the claim.
                                        
                                          How to Search:
                                            Give clear instructions for constructing search queries.
                                            Specify *how* to break down the claim into searchable components.
                                            If refining previous search, state what new filters, keywords, or source types to add.
                                        
                                          Example Queries (These are examples. The next agent can adapt them):
                                            - Provide 3–6 example search queries that reflect the search intent.
                                            - Include synonyms, alternate phrasings, and narrower variants as needed.
                                        
                                          Reliable Sources to Prioritize:
                                            Explicitly list which sources are more trustworthy for this claim type.
                                            Example categories:
                                              - Official organizational or corporate websites
                                              - Government documents and regulatory filings
                                              - Academic publications or recognized reference databases
                                              - Major reputable journalism outlets
                                            Also note sources that should NOT be relied on (e.g., unverified social posts, personal blogs, low-credibility forums).
                                        
                                          Validation Criteria for Summary:
                                            Describe how the next agent should decide whether the claim is:
                                              - Supported
                                              - Partially Supported / Uncertain
                                              - Not Supported
                                            Specify thresholds such as:
                                              - Number of independent sources required
                                              - Consistency of reported facts
                                              - Whether primary evidence is available
                                        
                                        Additional Context:
                                          Explain the purpose of clarifying this claim in relation to the post’s context.
                                          If referring to previous plans, summarize what has already been found and what this new plan adds.
                                        
                                        """;

    private const string AgentDescription = "The agent is responsible for generating a plan for analyzing post";

    public static readonly ChatClientAgentOptions Options = new()
    {
        Instructions = Instructions,
        Description = AgentDescription,
        ChatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                AIJsonUtilities.CreateJsonSchema(typeof(PlanGeneratorResponse))
                )
        }
    };

    public class PlanGeneratorResponse
    {
        public List<AnalyserPlan> Plans { get; set; } = [];
    }
}