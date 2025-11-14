using Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;
using Fakeoverflow.Backend.Worker.Agents.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

namespace Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;

public class ClaimExtractorAgent
{

    public const string ModelName = Constants.Models.BaseModel;
    
    private const string Instruction = """
                                       You are an expert text analyst. Your job is to read a forum post and do two things:

                                       1. Extract every claim made in the post.
                                       2. Identify the overall context of the discussion.

                                       A claim is any statement that asserts something about the world. Each claim must be labeled with one of the following types:

                                       - Factual: Can be proven true or false with evidence.
                                       - Opinion: A personal belief or subjective judgment.
                                       - Statistical: Refers to numbers, percentages, or measurable quantities.
                                       - Historical: Refers to past events or timelines.
                                       - Scientific: Based on scientific concepts, mechanisms, or research.

                                       Return each claim as a short, clear sentence without adding new information. 
                                       Do not rewrite claims into opinions or opinions into facts. 
                                       If no claims of a certain type are present, omit that type.

                                       Finally, summarize the context in 1–3 sentences explaining what the post is mainly about.
                                       """;

    
    private const string AgentDescription = "The agent is responsible for extracting claims and context from a post.";

    public static readonly ChatClientAgentOptions Options = new()
    {
        Name = nameof(ClaimExtractorAgent),
        Instructions = Instruction,
        Description = AgentDescription,
        ChatOptions = new ChatOptions()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(AIJsonUtilities.CreateJsonSchema(typeof(ClaimExtractorResponse), "")),
            Tools = [
                AIFunctionFactory.Create(CommonTools.GetCurrentTime)
            ]
        }
    };


    public class ClaimExtractorResponse
    {
        public List<Claim> ClaimsMade
        {
            get;
            set;
        } = [];

        public string Context { get; set; } = string.Empty;
    }
}